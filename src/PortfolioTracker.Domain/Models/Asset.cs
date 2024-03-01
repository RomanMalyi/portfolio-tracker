using System;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Domain.Models
{
    public class Asset
    {
        public string Id { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public AssetType AssetType { get; set; }
        public string? ExchangeTicker { get; set; }
        public decimal? OpenPrice { get; set; }
        public double? InterestRate { get; set; }
        public decimal Units { get; set; }
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
