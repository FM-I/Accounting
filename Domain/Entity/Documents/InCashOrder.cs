using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class InCashOrder : Document
    {
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; }
        public CashBox CashBox { get; set; }
    }
}
