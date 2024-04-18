﻿namespace Domain.Entity.Handbooks
{
    public class BankAccount : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public virtual Bank Bank { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (BankAccount)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }

    }
}
