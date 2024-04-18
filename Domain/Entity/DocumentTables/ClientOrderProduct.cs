using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DocumentTables
{
    public class ClientOrderProduct
    {
        public Guid Id { get; set; }
        public Nomenclature Nomenclature { get; set; }
        public Unit Unit { get; set; }
        public decimal Price {  get; set; }
        public decimal Summa { get; set; }
        public double Quantity { get; set; }
        [ForeignKey(nameof(ClientOrder))]
        public Guid IdClientOrder { get; set; }
        public virtual ClientOrder Document { get; set; }
    }
}
