using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Accumulations
{
    public class Purchase : IAccumulationRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        
        [ForeignKey(nameof(Nomenclature))]
        public Guid NomenclatureId { get; set; }
        public virtual Nomenclature Nomenclature { get; set; }

        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(Organization))]
        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public double Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
