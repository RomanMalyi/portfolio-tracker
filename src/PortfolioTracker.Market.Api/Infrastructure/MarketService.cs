using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PortfolioTracker.Events.Common;
using PortfolioTracker.Market.Api.Dto;

namespace PortfolioTracker.Market.Api.Infrastructure
{
    public class MarketService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Settings settings;

        //TODO: add cache
        private List<PolygonElement>? polygonElements;
        private List<CurrencyRate>? currencies;

        public MarketService(IOptions<Settings> polygonSettings, IHttpClientFactory httpClientFactory)
        {
            this.settings = polygonSettings.Value;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<List<PolygonElement>> Get()
        {
            if (polygonElements != null) return polygonElements;

            polygonElements = await GetMarket();
            return polygonElements;
        }

        public async Task<List<CurrencyRate>> GetCurrencies()
        {
            if (currencies != null) return currencies;

            var monoCurrencies = await GetMonobankCurrencies();
            currencies = monoCurrencies;

            //TODO: delete it when mono will return functionality for uah to usd converting
            var usdToUah = monoCurrencies.First(c => c.CurrencyA.Equals(Currency.USD) && c.CurrencyB.Equals(Currency.UAH));
            currencies.Add(new CurrencyRate()
            {
                CurrencyA = Currency.UAH,
                CurrencyB = Currency.USD,
                RateBuy = 1/usdToUah.RateBuy,
                RateSell = 1/usdToUah.RateSell
            });

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
            string date = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd");

            string cryptoUri = $"{settings.PolygonBaseUrl}/v2/aggs/grouped/locale/global/market/crypto/{date}?adjusted=true&apiKey={settings.PolygonApiKey}";
            //TODO: delete first and lust characters
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
