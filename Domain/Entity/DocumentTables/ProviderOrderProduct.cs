using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DocumentTables
{
    public class ProviderOrderProduct
    {
        public Guid Id { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        public virtual Unit Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Summa { get; set; }
        public double Quantity { get; set; }
        [ForeignKey(nameof(ProviderOrder))]
        public Guid IdProviderOrder { get; set; }
        public virtual ProviderOrder Document { get; set; }
    }
}
