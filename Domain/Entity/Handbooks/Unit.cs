
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Unit : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public double Coefficient { get; set; }

        public bool ChekOccupancy()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Code) && Coefficient > 0;
        }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Unit)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
