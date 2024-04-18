﻿
namespace Domain.Entity.Handbooks
{
    public class UnitClasificator : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (UnitClasificator)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
