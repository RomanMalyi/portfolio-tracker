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
        public bool IsEqual(string accountId)
        {
            return asset != null && asset.AccountId.Equals(accountId);
        }

        public Result<Asset, Error> Get()
        {
            if (asset == null)
            {
                return AssetErrors.AssetNotCreated;
            }

            return asset;
        }

        public Result<Asset, Error> Create(string id, string accountId,
            string userId, string name, Currency currency, double interestRate, double invested, RiskLevel riskLevel)
        {
            if (asset != null)
                return AssetErrors.AssetAlreadyCreated;

            Apply(new AssetCreated
            {
                //Id = Guid.NewGuid().ToString(), TODO: fix after testing
                AssetId = id,
                AccountId = accountId,
                UserId = userId,
                Currency = currency,
                InterestRate = interestRate,
                Invested = invested,
                Name = name,
                RiskLevel = riskLevel
            });

            return asset!;
        }

        private void When(AssetCreated @event)
        {
            asset = new Asset
            {
                Id = @event.AssetId,
                AccountId = @event.AccountId,
                UserId = @event.UserId,
                Name = @event.Name,
                Currency = @event.Currency,
                InterestRate = @event.InterestRate,
                Invested = @event.Invested,
                RiskLevel = @event.RiskLevel
            };
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
