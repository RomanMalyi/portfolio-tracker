using System;

namespace PortfolioTracker.Domain.Models
{
    public class Account
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        //TODO: do we need it
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public string UserId { get; set; } = null!;
    }

    public enum AccountType
    {
        CreditCard = 1,
        Cash = 2,
        CryptoWallet = 3,
        Deposit = 4,
        Loan = 5,
        Broker = 6,
        Other = 7
    }
}
