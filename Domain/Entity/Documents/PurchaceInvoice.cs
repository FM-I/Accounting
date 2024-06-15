using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Documents
{
    public class PurchaceInvoice : Document
    {
        public virtual TypePrice TypePrice { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<PurchaceInvoiceProduct> Products { get; set; } = new List<PurchaceInvoiceProduct>();
        public decimal Summa { get => Products.Sum(s => s.Summa); }
        public TypePurchaceInvoice TypeOperation { get; set; }
        public virtual Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        [ForeignKey(nameof(ProviderOrder))]
        public Guid? ProviderOrderId { get; set; }
        public virtual ProviderOrder? ProviderOrder { get; set; }

        [ForeignKey(nameof(ClientOrder))]
        public Guid? ClientOrderId { get; set; }
        public virtual ClientOrder? ClientOrder { get; set; }

        public override string ToString()
        {
            return $"Прибуткова накладна {Number} від {Date}";
        }

        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            List<IAccumulationRegister> leftovers = new();
            List<IAccumulationRegister> sales = new();
            List<IAccumulationRegister> purchase = new();

            foreach (var product in Products)
            {
                leftovers.Add(new Leftover()
                {
                    Date = DateTime.Now,
                    NomenclatureId = product.NomenclatureId,
                    Warehouse = Warehouse,
                    Value = product.Quantity,
                    TypeMove = TypeAccumulationRegisterMove.INCOMING
                });

                sales.Add(new Sale()
                {
                    Date = DateTime.Now,
                    NomenclatureId = product.NomenclatureId,
                    Client = Client,
                    Organization = Organization,
                    Price = product.Price,
                    Quantity = -product.Quantity,
                    Summa = product.Summa * (decimal)CurrencyRate
                });

                purchase.Add(new Purchase()
                {
                    Date = DateTime.Now,
                    NomenclatureId = product.NomenclatureId,
                    Client = Client,
                    Organization = Organization,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    Summa = product.Summa * (decimal)CurrencyRate
                });
            }

            moves.Add(typeof(Leftover), leftovers);

            if(TypeOperation == TypePurchaceInvoice.Buying)
            {

                var debts = new List<IAccumulationRegister>()
                {
                    new ProvidersDebt()
                    {
                        Provider = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = Summa * (decimal)CurrencyRate
                    }
                };

                moves.Add(typeof(ProvidersDebt), debts);
                moves.Add(typeof(Purchase), purchase);
            }
            else
            {
                var debts = new List<IAccumulationRegister>()
                {
                    new ClientsDebt()
                    {
                        Client = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = -Summa * (decimal)CurrencyRate
                    }
                };

                moves.Add(typeof(ClientsDebt), debts);
                moves.Add(typeof(Sale), sales);
            }

            return moves;
        }

        public override void FillWith(Document document)
        {
            if(document is ProviderOrder)
            {
                var data = (ProviderOrder)document;

                ProviderOrder = data;
                ProviderOrderId = data.Id;
                Client = data.Client;
                Organization = data.Organization;
                Warehouse = data.Warehouse;
                Currency = data.Currency;
                CurrencyRate = data.CurrencyRate;
                TypePrice = data.TypePrice;

                foreach (var product in data.Products)
                {
                    Products.Add(new()
                    {
                        Document = this,
                        DocumentId = Id,
                        Nomenclature = product.Nomenclature,
                        NomenclatureId = product.NomenclatureId,
                        UnitId = product.UnitId,
                        Price = product.Price,
                        Unit = product.Unit,
                        Quantity = product.Quantity,
                        Summa = product.Summa
                    });
                }

            }
            else if (document is ClientOrder)
            {
                var data = (ClientOrder)document;

                ClientOrder = data;
                ClientOrderId = data.Id;
                Client = data.Client;
                Organization = data.Organization;
                Warehouse = data.Warehouse;
                Currency = data.Currency;
                CurrencyRate = data.CurrencyRate;
                TypePrice = data.TypePrice;

                foreach (var product in data.Products)
                {
                    Products.Add(new()
                    {
                        Document = this,
                        DocumentId = Id,
                        Nomenclature = product.Nomenclature,
                        NomenclatureId = product.NomenclatureId,
                        UnitId = product.UnitId,
                        Price = product.Price,
                        Unit = product.Unit,
                        Quantity = product.Quantity,
                        Summa = product.Summa
                    });
                }
            }
        }

        public override Document DeepCopy()
        {
            PurchaceInvoice document = (PurchaceInvoice)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;
            document.Conducted = false;

            var products = new List<PurchaceInvoiceProduct>();

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
    }
}
