using System.Text.Json.Serialization;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.Asset
{
    public class CreateAsset
    {
        public string Name { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AssetType AssetType { get; set; }
        public string? ExchangeTicker { get; set; }
        public decimal? OpenPrice { get; set; }
        public double? InterestRate { get; set; }
        public decimal Units { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RiskLevel RiskLevel { get; set; }
    }
}
