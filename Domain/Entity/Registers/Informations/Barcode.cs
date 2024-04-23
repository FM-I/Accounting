using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class Barcode : IInformationRegister
    {
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }
        [ForeignKey(nameof(Unit))]
        public Guid UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        public string Value {  get; set; }
    }
}
