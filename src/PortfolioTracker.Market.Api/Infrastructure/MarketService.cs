using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PortfolioTracker.Market.Api.Dto;

namespace PortfolioTracker.Market.Api.Infrastructure
{
    public class MarketService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Settings settings;

        //TODO: add cache
        private readonly List<PolygonElement> polygonElements;
        private readonly List<CurrencyRate> currencies;

        public MarketService(IOptions<Settings> polygonSettings, IHttpClientFactory httpClientFactory)
        {
            this.settings = polygonSettings.Value;
            this.httpClientFactory = httpClientFactory;

            polygonElements = GetMarket().GetAwaiter().GetResult();
            currencies = GetMonobankCurrencies().GetAwaiter().GetResult();
        }

        public List<PolygonElement> Get()
        {
            return polygonElements;
        }

        public List<CurrencyRate> GetCurrencies()
        {
            return currencies;
        }

        private async Task<List<CurrencyRate>> GetMonobankCurrencies()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{settings.MonobankUrl}/bank/currency");

            var httpClient = httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            List<MonobankCurrency>? result = new List<MonobankCurrency>();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                result = await JsonSerializer.DeserializeAsync<List<MonobankCurrency>>(contentStream);
            }

            if (result == null) return new List<CurrencyRate>();

           return result.Where(CurrencyMapper.CanMap).Select(CurrencyMapper.Map).ToList();
        }


        private async Task<PolygonResponse> GetInfoFromPolygon(string uri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Headers =
                { { HeaderNames.Accept, "application/vnd.github.v3+json" } }
            };

            var httpClient = httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            PolygonResponse? result = new PolygonResponse();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                result = await JsonSerializer.DeserializeAsync
                    <PolygonResponse>(contentStream);
            }

            return result ?? new PolygonResponse();
        }

        private async Task<List<PolygonElement>> GetMarket()
        {
            string date = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            string cryptoUri = $"{settings.PolygonBaseUrl}/v2/aggs/grouped/locale/global/market/crypto/{date}?adjusted=true&apiKey={settings.PolygonApiKey}";
            var crypto = await GetInfoFromPolygon(cryptoUri);

            string stockUri = $"{settings.PolygonBaseUrl}/v2/aggs/grouped/locale/us/market/stocks/{date}?adjusted=true&apiKey={settings.PolygonApiKey}";
            var stock = await GetInfoFromPolygon(stockUri);

            var result = new List<PolygonElement>(crypto.ResultsCount + stock.ResultsCount);
            result.AddRange(stock.Results);
            result.AddRange(crypto.Results);

            return result;
        }
    }
}
