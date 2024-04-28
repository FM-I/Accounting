using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entity.Registers.Accumulations
{
    public class Leftover : IAccumulationRegister
    {
        [ForeignKey(nameof(Document))]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        [ForeignKey(nameof(Warehouse))]
        public Guid WarehouseId {  get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public double Value {  get; set; }
    }
}
