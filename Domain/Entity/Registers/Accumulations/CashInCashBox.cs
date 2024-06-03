using Domain.Entity.Handbooks;
using Domain.Enum;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Accumulations
{
    public class CashInCashBox : IAccumulationRegister, ILeftoverRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        public TypeAccumulationRegisterMove TypeMove { get; set; }

        [ForeignKey(nameof(CashBox))]
        public Guid CashBoxId { get; set; }
        public virtual CashBox CashBox { get; set; }

        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public decimal Summa { get; set; }
    }
}
