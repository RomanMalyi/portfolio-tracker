using System.Text.Json.Serialization;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Dto.Account
{
    public class CreateAccount
    {
        public string Name { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType AccountType { get; set; }
    }
}
