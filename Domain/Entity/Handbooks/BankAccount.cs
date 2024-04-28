using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class BankAccount : IHandbook, ICheckDataComplection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public virtual Bank Bank { get; set; }

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
            IHandbook handbook = (BankAccount)MemberwiseClone();
            handbook.Id = Guid.Empty;
            handbook.Code = String.Empty;
            return handbook;
        }

    }
}
