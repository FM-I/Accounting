
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Nomenclature : IHandbook, ICheckDataComplection
    {
        public Guid Id { get; set; }
        public string? Arcticle { get; set; }
        public Unit BaseUnit { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDefault { get; set; }
        public bool DeleteMark { get; set; }
        public virtual Nomenclature? Parent { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (string.IsNullOrWhiteSpace(Name))
                result.Properties.Add(nameof(Name));

            if (string.IsNullOrWhiteSpace(Code))
                result.Properties.Add(nameof(Code));

            if (!IsGroup)
            {
                if (string.IsNullOrWhiteSpace(Arcticle))
                    result.Properties.Add(nameof(Arcticle));

                if (BaseUnit == null)
                    result.Properties.Add(nameof(BaseUnit));
            }

            return result;
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
