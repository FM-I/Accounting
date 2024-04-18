using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DocumentTables
{
    public class SalesInvoiceProduct
    {
        public Guid Id { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        public virtual Unit Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Summa { get; set; }
        public double Quantity { get; set; }
        [ForeignKey(nameof(SaleInvoice))]
        public Guid IdProviderOrder { get; set; }
        public virtual SaleInvoice Document { get; set; }
    }
}
