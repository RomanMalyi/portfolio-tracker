namespace PortfolioTracker.Market.Api.Dto
{
    public class ShortResponse
    {
        public string Ticker { get; set; } = null!;
        public decimal ClosePrice { get; set; }
        public decimal Change { get; set; }
    }
}
