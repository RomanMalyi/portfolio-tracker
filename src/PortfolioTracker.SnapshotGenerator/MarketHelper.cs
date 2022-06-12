using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PortfolioTracker.SnapshotGenerator
{
    public class MarketHelper : IDisposable
    {
        private readonly HttpClient httpClient;

        public MarketHelper()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<ShortMarketResponse>> GetTickers(string[] tickers)
        {
            if (tickers.Length == 0) return new List<ShortMarketResponse>();
            //TODO: move url to settings
            string tickersUrl = tickers[0];
            if (tickers.Length > 1)
            {
                tickersUrl = string.Join("?tickers=", tickers);
            }
            var response = await httpClient.GetAsync($"https://localhost:7259/api/Market/tickers?tickers={tickersUrl}");
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ShortMarketResponse>>(responseBody);
        }

        public async Task<List<CurrencyRate>> GetCurrencies()
        {
            //TODO: move url to settings
            var response = await httpClient.GetAsync($"https://localhost:7259/api/Market/currencies");
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CurrencyRate>>(responseBody);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
