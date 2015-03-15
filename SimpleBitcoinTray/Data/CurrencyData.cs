using Info.Blockchain.API;
using Info.Blockchain.API.ExchangeRates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinTrayTicker.Data
{
    public static class CurrencyData
    {
        public static event EventHandler PriceUpdated = delegate { };

        public static ObservableCollection<History> PriceHistory = new ObservableCollection<History>();

        static Dictionary<string, Currency> _currentPrices = new Dictionary<string, Currency>();

        public static Dictionary<string, Currency> CurrentPrices
        {
            get { return _currentPrices; }
            set
            {
                _currentPrices = value;
            }
        }

        static Currency _selectedCurrency;

        public static Currency SelectedCurrency
        {
            get { return _selectedCurrency; }
            set
            {
                _selectedCurrency = value;
                PriceUpdated(null, EventArgs.Empty);
            }
        }

        public static async Task updatePricesAsync(string currency)
        {
            Task<Dictionary<string, Currency>> getPricesAsync = new Task<Dictionary<string, Currency>>(() =>
            {
                Dictionary<string, Currency> result = null;

                result = ExchangeRates.GetTicker(null);

                return result;

            });

            getPricesAsync.Start();

            var prices = await getPricesAsync;

            if (prices != null)
            {
                CurrentPrices = prices;

                PriceHistory.Add(new History(prices));

                SelectedCurrency = prices.SingleOrDefault(x => x.Key == currency).Value;
            }
        }

        public static async Task<double> convertToBTC(double value)
        {
            Task<double> getPriceAsync = new Task<double>(() =>
            {
                double result = 0;

                result = ExchangeRates.ToBTC(Properties.Settings.Default.SelectedCurrency, value, null);

                return result;

            });

            getPriceAsync.Start();

            var price = await getPriceAsync;

            return price;
        }

        public static History getSecondToLast(string currency)
        {
            if (PriceHistory.Count >= 2)
            {
                return PriceHistory[PriceHistory.Count - 2];
            }
            else
            {
                return null;
            }
        }

    }
}
