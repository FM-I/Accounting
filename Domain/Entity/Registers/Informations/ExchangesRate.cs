using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class ExchangesRate : IInformationRegister, IPeriodDateRegister, ICheckDataComplection
    {
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public double Rate { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (Date == default)
                result.Properties.Add("Дата");

            if (CurrencyId == Guid.Empty)
                result.Properties.Add("Валюта");

            if (Rate == 0)
                result.Properties.Add("Курс");

            return result;
        }

        public override bool Equals(object? obj)
        {
            return CurrencyId == ((ExchangesRate)obj).CurrencyId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CurrencyId);
        }
    }
}
