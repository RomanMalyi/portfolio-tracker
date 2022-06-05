namespace PortfolioTracker.Domain.Models
{
    internal class Asset
    {
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public double InterestRate { get; set; }
    }
}
