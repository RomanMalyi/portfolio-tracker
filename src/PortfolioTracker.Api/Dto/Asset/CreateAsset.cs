using System.Text.Json.Serialization;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.Asset
{
    public class CreateAsset
    {
        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RiskLevel RiskLevel { get; set; }
        public double Invested { get; set; }
        public double InterestRate { get; set; }
    }
}
