using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class ExchangesRate : IInformationRegister, IPeriodDateRegister
    {
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public double Rate { get; set; }

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
