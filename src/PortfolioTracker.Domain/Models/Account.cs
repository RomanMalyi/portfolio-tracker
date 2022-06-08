using System;

namespace PortfolioTracker.Domain.Models
{
    public class Account
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        //TODO: add missing properties
        public decimal Balance { get; set; }
        public AccountType AccountType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
