
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Handbooks
{
    public class Client : IHandbook, ICheckDataComplection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDefault { get; set; }
        public bool DeleteMark { get; set; }
        [ForeignKey(nameof(Parent))]
        public Guid? ParentId { get; set; } 
        public virtual Client? Parent { get; set; }
        public TypesClient TypeClient { get; set; } = TypesClient.None;

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (string.IsNullOrWhiteSpace(Name))
                result.Properties.Add("Найменування");

            if (!IsGroup)
            {
                if (TypeClient == TypesClient.None)
                    result.Properties.Add("Тип контрагента");
            }

            return result;
        }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Client)MemberwiseClone();
            handbook.Id = Guid.Empty;
            handbook.Code = String.Empty;
            handbook.IsDefault = false;
            handbook.DeleteMark = false;
            return handbook;
        }
    }
}
