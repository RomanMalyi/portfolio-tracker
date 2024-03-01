using PortfolioTracker.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.SnapshotGenerator
{
    public class AssetCalculator
    {
        private readonly List<CurrencyRate> currencies;
        private readonly List<ShortMarketResponse> marketValues;

        public AssetCalculator(List<CurrencyRate> currencies, List<ShortMarketResponse> marketValues)
        {
            this.currencies = currencies;
            this.marketValues = marketValues;
        }


        public decimal Sum(List<Asset> assets)
        {
            decimal totalAmount = 0;

            foreach (var asset in assets)
            {
                switch (asset.AssetType)
                {
                    case AssetType.Cryptocurrency:
                    case AssetType.Bonds:
                    case AssetType.Commodities:
                    case AssetType.Stocks:
                    case AssetType.Funds:
                        {
                            decimal marketValue = marketValues.FirstOrDefault(m => m.Ticker.Equals(asset.ExchangeTicker))?.ClosePrice ?? 1;

                            totalAmount += asset.Units * marketValue;
                            break;
                        }
                    case AssetType.CreditCard:
                    case AssetType.Deposit:
                    case AssetType.Money:
                    case AssetType.Other:
                    case AssetType.RealEstate:
                    case AssetType.DirectFinancing:
                        {
                            //TODO: is it ok to have double converting all the time?
                            decimal convertToUah = ConvertCurrency(currencies, asset.Currency, Currency.UAH, asset.Units);
                            totalAmount += ConvertCurrency(currencies, Currency.UAH, Currency.USD, convertToUah);
                            break;
                        }
                }
            }

            return totalAmount;
        }

        public decimal ConvertCurrency(List<CurrencyRate> currencies, Currency currencyFrom, Currency currencyTo, decimal unitsA)
        {
            var rate = currencies.FirstOrDefault(c => c.CurrencyA.Equals(currencyFrom) && c.CurrencyB.Equals(currencyTo));

            //TODO: how can I ensure all currencies?
            if (rate == null || currencyFrom.Equals(currencyTo)) return unitsA;

            return unitsA * rate.RateSell;
        }
    }
}
