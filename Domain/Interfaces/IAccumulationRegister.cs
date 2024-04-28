using Domain.Entity.Documents;
using Domain.Enum;

namespace Domain.Interfaces
{
    public interface IAccumulationRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
    }
}
