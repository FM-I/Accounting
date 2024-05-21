using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;

namespace Domain.Entity.Documents
{
    public class PurchaceInvoice : Document
    {
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<PurchaceInvoiceProduct> Products { get; set; } = new List<PurchaceInvoiceProduct>();
        public decimal Summa { get => Products.Sum(s => s.Summa); }
        public TypePurchaceInvoice TypeOperation { get; set; }
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual ProviderOrder? ProviderOrder { get; set; }

        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            List<IAccumulationRegister> leftovers = new();
            List<IAccumulationRegister> sales = new();

            foreach (var product in Products)
            {
                leftovers.Add(new Leftover()
                {
                    Date = DateTime.Now,
                    Nomenclature = product.Nomenclature,
                    Warehouse = Warehouse,
                    Value = product.Quantity / (product.Nomenclature.BaseUnit.Coefficient == 0 ? 1 : product.Nomenclature.BaseUnit.Coefficient),
                    TypeMove = Enum.TypeAccumulationRegisterMove.INCOMING
                });

                sales.Add(new Sale()
                {
                    Date = DateTime.Now,
                    Nomenclature = product.Nomenclature,
                    Client = Client,
                    Organization = Organization,
                    Price = product.Price,
                    Quantity = -product.Quantity
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
                Client = data.Client;
                Organization = data.Organization;
                Warehouse = data.Warehouse;

                foreach (var product in data.Products)
                {
                    Products.Add(new()
                    {
                        Document = this,
                        Nomenclature = product.Nomenclature,
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

            document.Products = [.. Products];

            return document;
        }
    }
}
