using Domain.Entity.Handbooks;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class Price : IInformationRegister, IPeriodDateRegister, ICheckDataComplection
    {
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }

        [ForeignKey(nameof(TypePrice))]
        public Guid TypePriceId { get; set; }
        public virtual TypePrice TypePrice { get; set; }

        public decimal Value { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (Date == default)
                result.Properties.Add("Дата");

            if (NomenclatureId == Guid.Empty)
                result.Properties.Add("Номенклатура");

            if (TypePriceId == Guid.Empty)
                result.Properties.Add("Тип ціни");

            if (Value == 0)
                result.Properties.Add("Ціна");

            return result;
        }

        public override bool Equals(object? obj)
        {
            Price other = obj as Price;
            return NomenclatureId == other.NomenclatureId && TypePriceId == other.TypePriceId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NomenclatureId, TypePriceId);
        }
    }
}
