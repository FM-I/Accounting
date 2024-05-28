using Domain.Entity.Handbooks;
using Domain.Enum;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Accumulations
{
    public class ClientsDebt : IAccumulationRegister, ILeftoverRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        public TypeAccumulationRegisterMove TypeMove { get; set; }
        
        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(Organization))]
        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        public decimal Value { get; set; }
    }
}
