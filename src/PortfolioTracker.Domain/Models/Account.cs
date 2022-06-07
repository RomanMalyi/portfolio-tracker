using System;

namespace PortfolioTracker.Domain.Models
{
    public class Account
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }

    public enum AccountType
    {
        Bank = 1,
        Broker = 2,
        CryptoWallet = 3,
        Cash = 4,
        Other = 5,
    }
}
