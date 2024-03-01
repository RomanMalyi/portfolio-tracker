using PortfolioTracker.Events.Common;
using PortfolioTracker.Market.Api.Dto;

namespace PortfolioTracker.Market.Api.Infrastructure
{
    public class CurrencyMapper
    {
        public static Dictionary<int, Currency> Currencies = new()
        {
            { 840, Currency.USD },
            { 980, Currency.UAH },
            { 978, Currency.EUR },
            { 985, Currency.PLN }
        };

        public static CurrencyRate Map(MonobankCurrency currency)
        {
            return new CurrencyRate()
            {
                CurrencyA = Currencies[currency.CurrencyCodeA],
                CurrencyB = Currencies[currency.CurrencyCodeB],
                Date = currency.Date,
                RateBuy = currency.RateBuy,
                RateSell = currency.RateSell,
            };
        }

        public static bool CanMap(MonobankCurrency currency)
        {
            return Currencies.ContainsKey(currency.CurrencyCodeA) && Currencies.ContainsKey(currency.CurrencyCodeB);
        }
    }
}
