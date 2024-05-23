using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DocumentTables
{
    public class SalesInvoiceProduct
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        [ForeignKey(nameof(Unit))]
        public Guid UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Summa { get; set; }
        public double Quantity { get; set; }
        [ForeignKey(nameof(Document))]
        public Guid DocumentId { get; set; }
        public virtual SaleInvoice Document { get; set; }
    }
}
