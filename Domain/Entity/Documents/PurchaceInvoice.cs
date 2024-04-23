using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class PurchaceInvoice : Document
    {
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<PurchaceInvoiceProduct> Products { get; set; }
    }
}
