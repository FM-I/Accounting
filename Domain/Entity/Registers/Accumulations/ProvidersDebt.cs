using Domain.Entity.Handbooks;
using Domain.Enum;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Accumulations
{
    public class ProvidersDebt : IAccumulationRegister, ILeftoverRegister
    {
        public DateTime Date { get; set; }
        public Guid DocumentId { get; set; }
        public TypeAccumulationRegisterMove TypeMove { get; set; }
        
        [ForeignKey(nameof(Provider))]
        public Guid ProviderId { get; set; }
        public Client Provider { get; set; }

        [ForeignKey(nameof(Organization))]
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public decimal Value { get; set; }
    }
}
