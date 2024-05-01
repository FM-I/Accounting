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
        public virtual ICollection<PurchaceInvoiceProduct> Products { get; set; }
        public decimal Summa { get => Products.Sum(s => s.Summa); }
        public TypePurchaceInvoice TypeDocument { get; set; }
        public virtual ProviderOrder? ProviderOrder { get; set; }

        public override Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            Dictionary<Type, List<IAccumulationRegister>> moves = new();

            List<IAccumulationRegister> leftovers = new List<IAccumulationRegister>();

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
            }

            moves.Add(typeof(Leftover), leftovers);

            if(TypeDocument == TypePurchaceInvoice.Buying)
            {

                var debts = new List<IAccumulationRegister>()
                {
                    new ProvidersDebt()
                    {
                        Provider = Client,
                        Organization = Organization,
                        Date = DateTime.Now,
                        TypeMove = Enum.TypeAccumulationRegisterMove.INCOMING,
                        Value = Summa
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
                        TypeMove = Enum.TypeAccumulationRegisterMove.INCOMING,
                        Value = -Summa
                    }
                };

                moves.Add(typeof(ClientsDebt), debts);
            }
            

            return moves;
        }
    }
}
