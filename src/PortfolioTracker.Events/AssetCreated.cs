using PortfolioTracker.Events.Common;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.Events
{
    [EventName(nameof(AssetCreated))]
    public class AssetCreated : IStoredEvent
    {
        public string AssetId { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public double Invested { get; set; }
        public double InterestRate { get; set; }
    }
}
