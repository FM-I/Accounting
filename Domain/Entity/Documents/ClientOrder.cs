using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class ClientOrder : Document
    {
        public TypePrice TypePrice { get; set; }
        public Currency Currency { get; set; }
        public Warehouse Warehouse { get; set; }
        public double CurrencyRate {  get; set; }
        public virtual ICollection<ClientOrderProduct> Products { get; set; }
    }
}
