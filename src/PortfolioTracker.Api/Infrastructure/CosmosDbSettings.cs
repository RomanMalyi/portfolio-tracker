namespace PortfolioTracker.Api.Infrastructure
{
    public class CosmosDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseId { get; set; } = null!;
    }
}
