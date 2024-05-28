
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Handbooks
{
    public class Nomenclature : IHandbook, ICheckDataComplection
    {
        public Guid Id { get; set; }
        public string? Arcticle { get; set; }
        [ForeignKey(nameof(BaseUnit))]
        public Guid? BaseUnitId { get; set; }
        public virtual Unit? BaseUnit { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDefault { get; set; }
        public bool DeleteMark { get; set; }
        [ForeignKey(nameof(Parent))]
        public Guid? ParentId { get; set; }
        public virtual Nomenclature? Parent { get; set; }
        public TypeNomenclature TypeNomenclature { get; set; } = TypeNomenclature.None;

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (string.IsNullOrWhiteSpace(Name))
                result.Properties.Add("Найменування");

            if (!IsGroup)
            {
                if (BaseUnit == null)
                    result.Properties.Add("Одиниця виміру");

                if (TypeNomenclature == TypeNomenclature.None
                    || TypeNomenclature == null)
                    result.Properties.Add("Тип номенклатури");
            }

            return result;
        }

        public IHandbook DeepCopy()
        {
            Nomenclature handbook = (Nomenclature)MemberwiseClone();
            handbook.Id = Guid.Empty;
            handbook.Code = String.Empty;
            handbook.IsDefault = false;
            handbook.DeleteMark = false;
            return handbook;
        }

        public override bool Equals(object? obj)
        {
            return Id == ((Nomenclature)obj).Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

    }
}
