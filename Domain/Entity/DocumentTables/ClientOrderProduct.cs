using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DocumentTables
{
    public class ClientOrderProduct
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        [ForeignKey(nameof(Unit))]
        public Guid UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        public decimal Price {  get; set; }
        public decimal Summa { get; set; }
        public double Quantity { get; set; }
        [ForeignKey(nameof(ClientOrder))]
        public Guid IdClientOrder { get; set; }
        public virtual ClientOrder Document { get; set; }
    }
}
