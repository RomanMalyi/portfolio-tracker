using System;
using PortfolioTracker.Events.Common;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.Events
{
    [EventName(nameof(TransactionAdded))]
    public class TransactionAdded : IStoredEvent
    {
        public TransactionAdded(string id, string assetId, string userId, TransactionType transactionType,
            DateTimeOffset transactionDate, decimal amount, string? fromAssetId, string? toAssetId,
            decimal? exchangeRate, string description, DateTimeOffset createdAt)
        {
            Id = id;
            AssetId = assetId;
            UserId = userId;
            TransactionType = transactionType;
            TransactionDate = transactionDate;
            Amount = amount;
            FromAssetId = fromAssetId;
            ToAssetId = toAssetId;
            ExchangeRate = exchangeRate;
            Description = description;
            CreatedAt = createdAt;
        }

        public string Id { get; }
        public string AssetId { get; }
        public string UserId { get; }
        public TransactionType TransactionType { get; }
        public DateTimeOffset TransactionDate { get; }
        public decimal Amount { get; }
        public string? FromAssetId { get; }
        public string? ToAssetId { get; }
        public decimal? ExchangeRate { get; }
        public string Description { get; }
        public DateTimeOffset CreatedAt { get; }
    }
}
