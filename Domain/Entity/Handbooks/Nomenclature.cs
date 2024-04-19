
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Nomenclature : IHandbook
    {
        public Guid Id { get; set; }
        public string? Arcticle { get; set; }
        public UnitClasificator? BaseUnit { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public virtual Nomenclature? Parent { get; set; }

        public bool ChekOccupancy()
        {
            if (IsGroup)
            {
                return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Code);
            }
            else
            {
                return !string.IsNullOrWhiteSpace(Name)
                    && !string.IsNullOrWhiteSpace(Code)
                    && !string.IsNullOrWhiteSpace(Arcticle)
                    && BaseUnit != null
                    && BaseUnit.Id != Guid.Empty;
            }

        }

        public IHandbook DeepCopy()
        {
            Nomenclature nomenclature = (Nomenclature)MemberwiseClone();
            nomenclature.Id = Guid.Empty;
            nomenclature.Code = string.Empty;
            return nomenclature;
        }
    }
}
