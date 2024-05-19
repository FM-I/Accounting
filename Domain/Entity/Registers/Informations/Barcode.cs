using Domain.Entity.Handbooks;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class Barcode : IInformationRegister, ICheckDataComplection
    {
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature? Nomenclature { get; set; }
        [ForeignKey(nameof(Unit))]
        public Guid UnitId { get; set; }
        public virtual Unit? Unit { get; set; }
        public string Value {  get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();
            if (NomenclatureId == Guid.Empty || NomenclatureId == null)
                result.Properties.Add("Номенклатура");

            if (UnitId == Guid.Empty || UnitId == null)
                result.Properties.Add("Одиниця виміру");

            if(string.IsNullOrWhiteSpace(Value))
                result.Properties.Add("Штрихкод");

            return result;
        }
    }
}
