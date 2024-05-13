using Domain.Interfaces;
using Domain.Models;

namespace Domain.Entity.Handbooks
{
    public interface IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDefault { get; set; }
        public bool DeleteMark { get; set; }
        public IHandbook DeepCopy();

    }
}
