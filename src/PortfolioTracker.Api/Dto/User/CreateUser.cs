using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.User
{
    public class CreateUser
    {
        public string Name { get; set; } = null!;
        public Currency MainCurrency { get; set; }
    }
}
