using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class ProviderOrder : Document
    {
        public TypePrice? TypePrice { get; set; }
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<ProviderOrderProduct> Products { get; set; }
        public decimal Summa { get => Products.Sum(s => s.Summa); }

        public override Document DeepCopy()
        {
            ProviderOrder document = (ProviderOrder)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;

            document.Products = [.. Products];

            return document;
        }
    }
}
