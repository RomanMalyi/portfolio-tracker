using System;

namespace PortfolioTracker.Domain.Models
{
    public class Account
    {
        public string Id { get; }

        public Account()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string? Name { get; set; }
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public double InterestRate { get; set; }
    }

    public enum AccountType
    {
        CreditCard = 1,
        Cash = 2,
        CrypoWallet = 3,
        Deposit = 4,
        Loan = 5,
        Broker = 6
    }
}
