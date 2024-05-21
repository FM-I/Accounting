using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;

namespace Domain.Entity.Documents
{
    public class SaleInvoice : Document
    {
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<SalesInvoiceProduct> Products { get; set; } = new List<SalesInvoiceProduct>();
        public decimal Summa { get => Products.Sum(s => s.Summa); }
        public TypeSaleInvoice TypeOperation { get; set; }
        public Currency Currency { get; set; }
        public double CurrencyRate { get; set; } = 1;
        public virtual ClientOrder? ClientOrder { get; set; }

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
                    TypeMove = TypeAccumulationRegisterMove.OUTCOMING
                });

                sales.Add(new Sale()
                {
                    Date = DateTime.Now,
                    Nomenclature = product.Nomenclature,
                    Client = Client,
                    Organization = Organization,
                    Price = product.Price,
                    Quantity = product.Quantity
                });
            }

            moves.Add(typeof(Leftover), leftovers);

            if (TypeOperation == TypeSaleInvoice.Sale)
            {
                moves.Add(typeof(Sale), sales);

                var debts = new List<IAccumulationRegister>()
                {
                    new ClientsDebt()
                    {
                        Client = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = Summa * (decimal)CurrencyRate
                    }
                };

                moves.Add(typeof(ClientsDebt), debts);
            }
            else
            {

                var debts = new List<IAccumulationRegister>()
                {
                    new ProvidersDebt()
                    {
                        Provider = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = TypeAccumulationRegisterMove.INCOMING,
                        Value = -Summa * (decimal)CurrencyRate
                    }
                };

                moves.Add(typeof(ProvidersDebt), debts);
            }

            return moves;
        }

        public override void FillWith(Document document)
        {
            if(document is ClientOrder)
            {
                var data = (ClientOrder)document;

                Client = data.Client;
                Organization = data.Organization;
                Warehouse = data.Warehouse;
                ClientOrder = data;

                foreach (var product in data.Products)
                {
                    Products.Add(new()
                    {
                        Document = this,
                        Nomenclature = product.Nomenclature,
                        Unit = product.Unit,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        Summa = product.Summa
                    });
                }
            }
        }

        public override Document DeepCopy()
        {
            SaleInvoice document = (SaleInvoice)MemberwiseClone();

            document.Id = Guid.Empty;
            document.Number = "";
            document.Date = DateTime.Now;
            document.DeleteMark = false;

            document.Products = [.. Products];

            return document;
        }
    }
}
