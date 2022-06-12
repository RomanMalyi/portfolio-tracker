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
            string tickersUrl = string.Join("&tickers=", tickers);
            tickersUrl = ReplaceFirstOccurrence(tickersUrl, "&", "?");
            var response = await httpClient.GetAsync($"https://localhost:7259/api/Market/tickers{tickersUrl}");
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

        private static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
