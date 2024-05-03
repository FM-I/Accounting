using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class ClientOrder : Document
    {
        public TypePrice? TypePrice { get; set; }
        public Currency Currency { get; set; }
        public Warehouse Warehouse { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual ICollection<ClientOrderProduct> Products { get; set; } = new List<ClientOrderProduct>();
        public decimal Summa { get => Products.Sum(s => s.Summa); }
    }
}
