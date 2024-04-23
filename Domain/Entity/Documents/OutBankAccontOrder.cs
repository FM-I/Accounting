using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class OutBankAccontOrder : Document
    {
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; }
        public BankAccount Account { get; set; }
    }
}
