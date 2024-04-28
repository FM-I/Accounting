
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Contact : IHandbook, ICheckDataComplection
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (!string.IsNullOrWhiteSpace(Name))
                result.Properties.Add(nameof(Name));

            if (!string.IsNullOrWhiteSpace(Code))
                result.Properties.Add(nameof(Code));

            return result;
        }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Contact)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
