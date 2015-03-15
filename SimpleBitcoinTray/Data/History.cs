using Info.Blockchain.API.ExchangeRates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinTrayTicker.Data
{
    public class History
    {
        public DateTime Time;

        public Dictionary<string, Currency> Prices;

        public History(Dictionary<string, Currency> newPrices)
        {

            Prices = newPrices;
            Time = DateTime.Now;
        }
    }
}
