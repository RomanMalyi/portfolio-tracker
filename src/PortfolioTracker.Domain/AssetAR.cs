using System;
using CSharpFunctionalExtensions;
using PortfolioTracker.Domain.Errors;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events;
using PortfolioTracker.Events.Common;
using PortfolioTracker.EventStore.Core;
using System.Collections.Generic;

namespace PortfolioTracker.Domain
{
    // ReSharper disable once InconsistentNaming
    public class AssetAR : AggregateRoot
    {
        public long Version { get; }

        private Asset? asset;

        public AssetAR(IReadOnlyCollection<IStoredEvent>? events = null, long version = 0)
        {
            if (events == null) return;

            Load(events);
            Version = version;
        }

        //TODO: remove after connect to the CosmosDB
        public bool IsAccountEqual(string accountId)
        {
            return asset != null && asset.AccountId.Equals(accountId);
        }

        //TODO: remove after connect to the CosmosDB
        public bool IsAssetEqual(string assetId)
        {
            return asset != null && asset.Id.Equals(assetId);
        }

        public Result<Asset, Error> Get()
        {
            if (asset == null)
                return AssetErrors.AssetNotCreated;


            return asset;
        }

        //TODO: remove id after tests finish
        public Result<Asset, Error> Create(string id, string accountId, string userId,
            string name, AssetType assetType, string? exchangeTicker, decimal? openPrice,
            Currency currency, double? interestRate, decimal units, RiskLevel riskLevel)
        {
            if (asset != null)
                return AssetErrors.AssetAlreadyCreated;

            Apply(new AssetCreated(id, accountId, userId, name, assetType, exchangeTicker, openPrice, interestRate, units, currency, riskLevel,
                DateTimeOffset.UtcNow));

            return asset!;
        }

        public Result<TransactionAdded, Error> AddTransaction(string assetId, string userId,
            decimal amount, string description, TransactionType transactionType, DateTimeOffset transactionDate,
            string? fromAssetId, string? toAssetId, decimal? exchangeRate)
        {
            if (asset == null)
                return AssetErrors.AssetNotCreated;

            //TODO: add validation for amount>0, userid = asset userId, transfer <=0 fromId and toId etc.

            //TODO: return basket here after transaction read model is created
            var @event = new TransactionAdded(Guid.NewGuid().ToString(), assetId, userId, transactionType,
                transactionDate, amount, fromAssetId, toAssetId, exchangeRate, description, DateTimeOffset.UtcNow);
            Apply(@event);

            return @event;
        }

        private void When(AssetCreated @event)
        {
            asset = new Asset
            {
                Id = @event.Id,
                AccountId = @event.AccountId,
                UserId = @event.UserId,
                Name = @event.Name,
                AssetType = @event.AssetType,
                ExchangeTicker = @event.ExchangeTicker,
                OpenPrice = @event.OpenPrice,
                InterestRate = @event.InterestRate,
                Units = @event.Units,
                Currency = @event.Currency,
                RiskLevel = @event.RiskLevel,
                CreatedAt = @event.CreatedAt
            };
        }

        private void When(TransactionAdded @event)
        {
            switch (@event.TransactionType)
            {
                case TransactionType.Expense:
                    {
                        asset!.Units -= @event.Amount;
                        break;
                    }
                case TransactionType.Income:
                    {
                        asset!.Units += @event.Amount;
                        break;
                    }
                case TransactionType.Transfer:
                    {
                        if (asset!.Id.Equals(@event.FromAssetId, StringComparison.OrdinalIgnoreCase))
                        {
                            asset.Units -= @event.Amount;
                            break;
                        }
                        asset.Units += @event.Amount;
                        break;
                    }
                default:
                    throw new Exception("Transaction type is not supported.");
            }
        }

        private void Load(IEnumerable<IStoredEvent> events)
        {
            foreach (var @event in events)
            {
                Mutate(@event);
            }
        }

        protected override void Mutate(IStoredEvent @event)
        {
            When((dynamic)@event);
        }
    }
}
