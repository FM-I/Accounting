using Domain.Entity.Documents;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Accumulations
{
    public class Sale : IAccumulationRegister
    {
        public DateTime Date { get; set; }
        [ForeignKey(nameof(Document))]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
