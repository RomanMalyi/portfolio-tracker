using System.Text.Json.Serialization;

namespace PortfolioTracker.Market.Api.Dto
{
    public class MonobankCurrency
    {
        [JsonPropertyName("currencyCodeA")]
        public int CurrencyCodeA { get; set; }
        [JsonPropertyName("currencyCodeB")]
        public int CurrencyCodeB { get; set; }
        [JsonPropertyName("date")]
        public long Date { get; set; }
        [JsonPropertyName("rateBuy")]
        public decimal RateBuy { get; set; }
        [JsonPropertyName("rateSell")]
        public decimal RateSell { get; set; }
    }
}
