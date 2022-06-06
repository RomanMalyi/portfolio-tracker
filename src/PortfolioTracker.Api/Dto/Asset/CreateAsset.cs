using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.Asset
{
    public class CreateAsset
    {
        public string Name { get; set; } = null!;
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public double Invested { get; set; }
        public double InterestRate { get; set; }
    }
}
