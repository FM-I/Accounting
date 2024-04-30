using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Interfaces;

namespace Domain.Entity.Documents
{
    public class SaleInvoice : Document
    {
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<SalesInvoiceProduct> Products { get; set; }

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
                    TypeMove = Enum.TypeAccumulationRegisterMove.OUTCOMING
                });
            }

            moves.Add(typeof(Leftover), leftovers);

            return moves;
        }
    }
}
