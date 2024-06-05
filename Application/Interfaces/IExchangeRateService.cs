using BL.Common;

namespace BL.Interfaces
{
    public interface IExchangeRateService
    {
        Task<List<ExangeRate>> GetExchangeRatesAsync(string currencyName = "");
    }
}
