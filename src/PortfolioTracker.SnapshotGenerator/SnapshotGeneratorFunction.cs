using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Domain.Models;

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

            string sqlConnectionString = Environment.GetEnvironmentVariables()["SqlDbConnectionString"] as string;
            SqlDatabase sqlDatabase = new SqlDatabase(sqlConnectionString);
            AssetRepository assetRepository = new(sqlDatabase);
            AccountRepository accountRepository = new(sqlDatabase);

            var assetsResult = await assetRepository.GetByUser(trigger.UserId, 0, 10000);

            //var accountsResult =

            Snapshot snapshot = new Snapshot()
            {
                Id = $"{trigger.UserId}-{trigger.Date:yyyy-MM-dd}",
                UserId = trigger.UserId,
                GenerationTime = DateTimeOffset.UtcNow,
                CurrencyAnalytics = GenerateCurrencyAnalytics(assetsResult.Data),
                //AccountAnalytics = GenerateAccountAnalytics(assetsResult.Data),
                //AssetTypeAnalytics = GenerateAssetTypeAnalytics(assetsResult.Data),
                //RiskLevelAnalytics = GenerateRiskLevelAnalytics(assetsResult.Data),
                //TransactionTypeAnalytics = GenerateTransactionTypeAnalytics(assetsResult.Data)
            };

            await snapshotRepository.Upsert(snapshot);

        }

        private List<CurrencyAnalytics> GenerateCurrencyAnalytics(List<Asset> assets)
        {
            var totalAssetsValue = assets.Sum(a => a.Units);
            var groupByCurrency = assets.GroupBy(a => a.Currency);
            var result = new List<CurrencyAnalytics>(groupByCurrency.Count());
            foreach (var group in groupByCurrency)
            {
                var groupTotal = group.Sum(g => g.Units);

                result.Add(new CurrencyAnalytics()
                {
                    Currency = group.Key,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalAssetsValue)
                });
            }

            return result;
        }

        private List<AccountAnalytics> GenerateAccountAnalytics(List<Asset> assets)
        {
            //TODO: add logic for converting to main currency
            var totalAssetsValue = assets.Sum(a => a.Units);
            var groupByCurrency = assets.GroupBy(a => a.AccountId);
            var result = new List<AccountAnalytics>(groupByCurrency.Count());
            foreach (var group in groupByCurrency)
            {
                var groupTotal = group.Sum(g => g.Units);

                result.Add(new AccountAnalytics()
                {
                    AccountId = group.Key,
                    //AccountType = ,
                    PortfolioAmount = groupTotal,
                    PortfolioPercent = (float)(groupTotal / totalAssetsValue)
                });
            }

            return result;
        }

        private List<AssetTypeAnalytics> GenerateAssetTypeAnalytics(List<Asset> assets)
        {
            throw new NotImplementedException();
        }

        private List<RiskLevelAnalytics> GenerateRiskLevelAnalytics(List<Asset> assets)
        {
            throw new NotImplementedException();
        }

        private List<TransactionTypeAnalytics> GenerateTransactionTypeAnalytics(List<Asset> assets)
        {
            throw new NotImplementedException();
        }
    }
}
