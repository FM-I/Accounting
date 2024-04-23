using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class Price : IInformationRegister, IPeriodDateRegister
    {
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }

        [ForeignKey(nameof(TypePrice))]
        public Guid TypePriceId { get; set; }
        public virtual TypePrice TypePrice { get; set; }

        public decimal Value { get; set; }

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
