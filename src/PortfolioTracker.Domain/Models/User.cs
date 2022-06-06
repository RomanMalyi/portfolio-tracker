using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Domain.Models
{
    public class User
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Currency MainCurrency { get; set; }
    }
}
