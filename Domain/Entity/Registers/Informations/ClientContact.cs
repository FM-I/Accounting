using Domain.Entity.Handbooks;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Registers.Informations
{
    public class ClientContact : IInformationRegister
    {
        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Contact))]
        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
