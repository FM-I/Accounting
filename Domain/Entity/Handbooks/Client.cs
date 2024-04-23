﻿
using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Handbooks
{
    public class Client : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(9)]
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public Client? Parent { get; set; }
        public TypesClient TypeClient { get; set; }
        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Client)MemberwiseClone();
            handbook.Id = Guid.Empty;
            handbook.Code = String.Empty;
            return handbook;
        }
    }
}
