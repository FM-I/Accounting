using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class SaleInvoice : Document
    {
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<SalesInvoiceProduct> Products { get; set; }
    }
}
