﻿
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Organization : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Organization)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
