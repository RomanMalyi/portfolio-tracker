using System.Text.Json.Serialization;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.Transaction
{
    public class AddTransaction
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? FromAssetId { get; set; }
        public string? ToAssetId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string Description { get; set; } = null!;
    }
}
