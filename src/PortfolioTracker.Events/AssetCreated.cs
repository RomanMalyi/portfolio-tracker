using System;
using PortfolioTracker.Events.Common;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.Events
{
    [EventName(nameof(AssetCreated))]
    public class AssetCreated : IStoredEvent
    {
        public AssetCreated(string id, string accountId, string userId, string name, AssetType assetType,
            string? exchangeTicker, decimal? openPrice, double? interestRate, decimal units,
            Currency currency, RiskLevel riskLevel, DateTimeOffset createdAt)
        {
            Id = id;
            AccountId = accountId;
            UserId = userId;
            Name = name;
            AssetType = assetType;
            ExchangeTicker = exchangeTicker;
            OpenPrice = openPrice;
            InterestRate = interestRate;
            Units = units;
            Currency = currency;
            RiskLevel = riskLevel;
            CreatedAt = createdAt;
        }

        public string Id { get; }
        public string AccountId { get; }
        public string UserId { get; }
        public string Name { get; }
        public AssetType AssetType { get; }
        public string? ExchangeTicker { get; }
        public decimal? OpenPrice { get; }
        public double? InterestRate { get; }
        public decimal Units { get; }
        public Currency Currency { get; }
        public RiskLevel RiskLevel { get; }
        public DateTimeOffset CreatedAt { get; }
    }
}
