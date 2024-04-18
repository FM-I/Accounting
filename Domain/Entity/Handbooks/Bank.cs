namespace Domain.Entity.Handbooks
{
    public class Bank : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Bank)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
