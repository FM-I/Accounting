﻿
namespace Domain.Entity.Handbooks
{
    public class Organization : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            return (Organization)MemberwiseClone();
        }
    }
}
