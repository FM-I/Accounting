using Domain.Entity.Handbooks;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Documents
{
    public abstract class Document : ICheckDataComplection
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool Conducted { get; set; }
        [MaxLength(9)]
        public string Number { get; set; }
        public virtual Client Client { get; set; }
        public virtual Organization Organization { get; set; }
        public bool DeleteMark { get; set; }

        public virtual void FillWith(Document document) {}

        public virtual Dictionary<Type, List<IAccumulationRegister>> GetAccumulationMove()
        {
            return new Dictionary<Type, List<IAccumulationRegister>>();
        }

        public abstract Document DeepCopy();
        public abstract DataComplectionResult CheckDataComplection();

    }
}
