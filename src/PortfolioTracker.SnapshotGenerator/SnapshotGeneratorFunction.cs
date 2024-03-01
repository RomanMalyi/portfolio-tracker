using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.SnapshotGenerator
{
    public class SnapshotGeneratorFunction
    {
        [FunctionName(nameof(SnapshotGeneratorFunction))]
        public async Task Run([ServiceBusTrigger("snapshot-trigger", Connection = "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var trigger = JsonConvert.DeserializeObject<SnapshotGenerationTrigger>(myQueueItem);
            if (trigger == null) throw new ArgumentException("Unexpected event type");

            string cosmosConnectionString = Environment.GetEnvironmentVariables()["CosmosDbConnectionString"] as string;
            using CosmosClient cosmosClient = new CosmosClient(cosmosConnectionString);
            SnapshotRepository snapshotRepository = new SnapshotRepository(cosmosClient);
            var eventStoreSerializer = new JsonStoredEventSerializer(typeof(AssetCreated).Assembly);
            var eventStore = new AssetEventStore(eventStoreSerializer, cosmosClient.GetDatabase("PortfolioTracker"));
            AssetArRepository assetArRepository = new(eventStore);

            string sqlConnectionString = Environment.GetEnvironmentVariables()["SqlDbConnectionString"] as string;
            SqlDatabase sqlDatabase = new SqlDatabase(sqlConnectionString);
            AssetRepository assetRepository = new(sqlDatabase);
            AccountRepository accountRepository = new(sqlDatabase);

            var currentAssetsResult = await assetRepository.GetByUser(trigger.UserId, 0, 10000);
            var accountsResult = await accountRepository.Get(trigger.UserId, 0, 1000);

            List<Asset> assetsForSnapshot = currentAssetsResult.Data;
            //Get assets for some historical moment
            if (DateOnly.FromDateTime(trigger.Date) < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                List<Task<Maybe<AssetAR>>> oldAssetsTasks = new List<Task<Maybe<AssetAR>>>(currentAssetsResult.Data.Count);
                oldAssetsTasks.AddRange(currentAssetsResult.Data.Select(currentAsset => assetArRepository.GetByDate(currentAsset.Id, trigger.Date)));
                List<Maybe<AssetAR>> oldAssets = (await Task.WhenAll(oldAssetsTasks)).ToList();
                assetsForSnapshot = oldAssets
                    .Where(e => e.HasValue)
                    .Select(a => a.Value.Get())
                    .Where(i=>i.IsSuccess)
                    .Select(i=>i.Value)
                    .ToList();
            }

            string[] allUsedTickers = currentAssetsResult.Data.Where(a => a.ExchangeTicker != null).Select(a => a.ExchangeTicker).Distinct().ToArray();

            MarketHelper marketHelper = new();
            List<CurrencyRate> currencies = await marketHelper.GetCurrencies();
            List<ShortMarketResponse> marketValues = await marketHelper.GetTickers(allUsedTickers);
            AssetCalculator assetCalculator = new AssetCalculator(currencies, marketValues);

            var totalAssetsValue = assetCalculator.Sum(assetsForSnapshot);

            Snapshot snapshot = new Snapshot()
            {
                Id = $"{trigger.UserId}-{trigger.Date:yyyy-MM-dd}",
                UserId = trigger.UserId,
                GenerationTime = DateTimeOffset.UtcNow,
                SnapshotDate = trigger.Date,
                TotalAmount = totalAssetsValue,
                CurrencyAnalytics = GenerateCurrencyAnalytics(assetCalculator, assetsForSnapshot, totalAssetsValue),
                AccountAnalytics = GenerateAccountAnalytics(assetCalculator, assetsForSnapshot, accountsResult.Data, totalAssetsValue),
                AssetTypeAnalytics = GenerateAssetTypeAnalytics(assetCalculator, assetsForSnapshot, totalAssetsValue),
                RiskLevelAnalytics = GenerateRiskLevelAnalytics(assetCalculator, assetsForSnapshot, totalAssetsValue)
            };

            await snapshotRepository.Upsert(snapshot);
        }

        private List<CurrencyAnalytics> GenerateCurrencyAnalytics(AssetCalculator assetCalculator, List<Asset> assets, decimal totalPortfolioValue)
        {
            var groupByCurrency = assets.GroupBy(a => a.Currency);
            var result = new List<CurrencyAnalytics>(groupByCurrency.Count());
            foreach (var group in groupByCurrency)
            {
                var groupTotal = assetCalculator.Sum(group.ToList());

                result.Add(new CurrencyAnalytics()
                {
                    Currency = group.Key,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalPortfolioValue)
                });
            }

            return result;
        }

        private List<AccountAnalytics> GenerateAccountAnalytics(AssetCalculator assetCalculator, List<Asset> assets, List<Account> accounts, decimal totalPortfolioValue)
        {
            var groupByAccount = assets.GroupBy(a => a.AccountId);
            var result = new List<AccountAnalytics>(groupByAccount.Count());
            foreach (var group in groupByAccount)
            {
                var groupTotal = assetCalculator.Sum(group.ToList());
                var account = accounts.FirstOrDefault(a => a.Id.Equals(group.Key, StringComparison.OrdinalIgnoreCase));
                if (account == null) continue;

                result.Add(new AccountAnalytics()
                {
                    AccountId = group.Key,
                    AccountType = account.AccountType,
                    AccountName = account.Name,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalPortfolioValue)
                });
            }

            return result;
        }

        private List<AssetTypeAnalytics> GenerateAssetTypeAnalytics(AssetCalculator assetCalculator, List<Asset> assets, decimal totalPortfolioValue)
        {
            var groupByAssetType = assets.GroupBy(a => a.AssetType);
            var result = new List<AssetTypeAnalytics>(groupByAssetType.Count());
            foreach (var group in groupByAssetType)
            {
                var groupTotal = assetCalculator.Sum(group.ToList());

                result.Add(new AssetTypeAnalytics()
                {
                    AssetType = group.Key,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalPortfolioValue)
                });
            }

            return result;
        }

        private List<RiskLevelAnalytics> GenerateRiskLevelAnalytics(AssetCalculator assetCalculator, List<Asset> assets, decimal totalPortfolioValue)
        {
            var groupByRiskLevel = assets.GroupBy(a => a.RiskLevel);
            var result = new List<RiskLevelAnalytics>(groupByRiskLevel.Count());
            foreach (var group in groupByRiskLevel)
            {
                var groupTotal = assetCalculator.Sum(group.ToList());

                result.Add(new RiskLevelAnalytics()
                {
                    RiskLevel = group.Key,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalPortfolioValue)
                });
            }

            return result;
        }
    }
}
