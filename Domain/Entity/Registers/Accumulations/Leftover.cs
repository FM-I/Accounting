using Domain.Entity.Handbooks;
using Domain.Enum;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entity.Registers.Accumulations
{
    public class Leftover : IAccumulationRegister, ILeftoverRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        public TypeAccumulationRegisterMove TypeMove { get; set; }

        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        
        [ForeignKey(nameof(Warehouse))]
        public Guid WarehouseId {  get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public double Value {  get; set; }
    }
}
