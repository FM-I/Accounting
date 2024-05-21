using Domain.Entity.Handbooks;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Entity.Registers.Informations
{
    public class ClientContact : IInformationRegister, ICheckDataComplection
    {
        [ForeignKey(nameof(Client))]
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Contact))]
        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (ClientId == Guid.Empty)
                result.Properties.Add("Контрагент");

            if (ContactId == Guid.Empty)
                result.Properties.Add("Контакт");

            return result;
        }
    }
}
