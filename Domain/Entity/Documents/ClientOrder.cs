using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Models;

namespace Domain.Entity.Documents
{
    public class ClientOrder : Document
    {
        public virtual TypePrice? TypePrice { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual ICollection<ClientOrderProduct> Products { get; set; } = new List<ClientOrderProduct>();
        public decimal Summa { get => Products.Sum(s => s.Summa); }

        public override DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (Client == null)
                result.Properties.Add("Контрагент");

            if (Organization == null)
                result.Properties.Add("Організація");

            if (Warehouse == null)
                result.Properties.Add("Склад");

            if (TypePrice == null)
                result.Properties.Add("Тип ціни");

            if (Currency == null)
                result.Properties.Add("Валюта");

            return result;
        }

        public override Document DeepCopy()
        {
            ClientOrder document = (ClientOrder)MemberwiseClone();
            
            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;
            document.Conducted = false;

            var products = new List<ClientOrderProduct>();

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

        public override string ToString()
        {
            return $"Замовлення покупця {Number} від {Date}";
        }
    }
}
