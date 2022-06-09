using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace PortfolioTracker.Api.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CosmosDbSettings>()
                .Bind(configuration.GetSection("CosmosDb"));

            services.AddHttpClient();
            services.AddSingleton<CosmosClient>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var settings = provider.GetRequiredService<IOptions<CosmosDbSettings>>().Value;
                var cosmosClientOptions = new CosmosClientOptions
                {
                    HttpClientFactory = httpClientFactory.CreateClient
                };

                return new CosmosClient(settings.ConnectionString, cosmosClientOptions);
            });

            return services;
        }
    }
}
