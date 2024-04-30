using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Documents
{
    public abstract class Document
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool Conducted { get; set; }
        [MaxLength(9)]
        public string Number { get; set; }
        public Client Client { get; set; }
        public Organization Organization { get; set; }

        public void WriteData() { }
        public virtual Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            return new Dictionary<Type, List<IAccumulationRegister>>();
        }
    }
}
