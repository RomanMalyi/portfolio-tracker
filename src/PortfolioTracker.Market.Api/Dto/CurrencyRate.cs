using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Market.Api.Dto
{
    public class CurrencyRate
    {
        public Currency CurrencyA { get; set; }
        public Currency CurrencyB { get; set; }
        public long Date { get; set; }
        public decimal RateBuy { get; set; }
        public decimal RateSell { get; set; }
    }
}
