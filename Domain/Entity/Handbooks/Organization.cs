
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Organization : IHandbook, ICheckDataComplection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public bool DeleteMark { get; set; }
        public bool IsDefault { get; set; }
        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (string.IsNullOrWhiteSpace(Name))
                result.Properties.Add("Найменування");

            return result;
        }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Organization)MemberwiseClone();
            handbook.Id = Guid.Empty;
            handbook.Code = String.Empty;
            handbook.DeleteMark = false;
            return handbook;
        }
    }
}
