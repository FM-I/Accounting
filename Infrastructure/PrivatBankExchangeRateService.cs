using BL.Common;
using BL.Interfaces;
using System.Globalization;
using System.Net.Http.Json;

namespace Infrastructure
{
    public class PrivatBankExchangeRateService : IExchangeRateService
    {
        private record Data(string? ccy, string? base_ccy, string? buy, string? sale);

        public async Task<List<ExangeRate>> GetExchangeRatesAsync(string currencyName = "")
        {
            List<ExangeRate> rates = new List<ExangeRate>();
            var uri = new Uri(@"https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            try
            {
                var result = await client.GetFromJsonAsync<List<Data>>(uri);

                if (result == null)
                    return rates;

                foreach(var item in result)
                {
                    if (!string.IsNullOrWhiteSpace(currencyName) && item.ccy != currencyName) continue;

                    var rate = new ExangeRate()
                    {
                        CurrengeBaseName = item.base_ccy,
                        CurrenyName = item.ccy,
                    };

                    double sale = 0;
                    double buy = 0;

                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                    double.TryParse(item.sale, NumberStyles.AllowDecimalPoint, culture, out sale);
                    double.TryParse(item.buy, NumberStyles.AllowDecimalPoint, culture, out buy);

                    rate.CurrengeBuyRate = buy;
                    rate.CurrengeSaleRate = sale;

                    rates.Add(rate);
                }

            }
            catch (Exception) {}

            return rates;
        }
    }
}
