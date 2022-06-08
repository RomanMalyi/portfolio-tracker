using System;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Domain.Models
{
    public class Transaction
    {
        public string Id { get; set; } = null!;
        public string AssetId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public TransactionType TransactionType { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? FromAssetId { get; set; }
        public string? ToAssetId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string Description { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
