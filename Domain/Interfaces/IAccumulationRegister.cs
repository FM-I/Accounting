using Domain.Entity.Documents;

namespace Domain.Interfaces
{
    public interface IAccumulationRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
