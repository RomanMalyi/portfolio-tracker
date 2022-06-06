using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Dto.Account
{
    public class CreateAccount
    {
        public string Name { get; set; } = null!;
        public AccountType AccountType { get; set; }
    }
}
