using System.Text.Json.Serialization;

namespace PortfolioTracker.Market.Api.Dto
{
    public class PolygonResponse
    {
        [JsonPropertyName("queryCount")]
        public long QueryCount { get; set; }
        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;
        [JsonPropertyName("results")]
        public List<PolygonElement> Results { get; set; } = null!;
    }

    public class PolygonElement
    {
        [JsonPropertyName("T")]
        public string Ticker { get; set; } = null!;
        [JsonPropertyName("c")]
        public decimal ClosePrice { get; set; }
        [JsonPropertyName("h")]
        public decimal HighestPrice { get; set; }
        [JsonPropertyName("l")]
        public decimal LowestPrice { get; set; }
        [JsonPropertyName("n")]
        public long NumberOfTransactions { get; set; }
        [JsonPropertyName("o")]
        public decimal OpenPrice { get; set; }
        [JsonPropertyName("v")]
        public decimal TradingVolume { get; set; }
        [JsonPropertyName("vw")]
        public decimal VolumeWeightedAveragePrice { get; set; }
    }
}
