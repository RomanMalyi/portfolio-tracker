using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto
{
    public class AnalyticsResponse
    {
        public string MostPopularCurrency { get; set; }
        public string HighestLevelOfRisk { get; set; }
        public int NumberOfAsstTypes { get; set; }
        public int NumberOfAccounts { get; set; }
        public decimal CurrentTotalAmount { get; set; }
        public decimal PortfolioChange { get; set; }
        public List<Snapshot> Snapshots { get; set; } = null!;
    }
}
