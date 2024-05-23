using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Models;

namespace Domain.Entity.Documents
{
    public class ProviderOrder : Document
    {
        public virtual TypePrice? TypePrice { get; set; }
        public virtual Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<ProviderOrderProduct> Products { get; set; }
        public decimal Summa { get => Products.Sum(s => s.Summa); }

        public override DataComplectionResult CheckDataComplection()
        {
            throw new NotImplementedException();
        }

        public override Document DeepCopy()
        {
            ProviderOrder document = (ProviderOrder)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;
            document.Conducted = false;

            var products = new List<ProviderOrderProduct>();

            foreach (var item in Products)
            {
                products.Add(new()
                {
                    Nomenclature = item.Nomenclature,
                    Unit = item.Unit,
                    NomenclatureId = item.NomenclatureId,
                    UnitId = item.UnitId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Summa = item.Summa
                });
            }

            document.Products = products;

            return document;
        }
    }
}
