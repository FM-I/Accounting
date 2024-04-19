
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class TypePrice : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (TypePrice)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
