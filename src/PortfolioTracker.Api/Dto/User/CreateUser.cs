using System.Text.Json.Serialization;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Api.Dto.User
{
    public class CreateUser
    {
        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency MainCurrency { get; set; }
    }
}
