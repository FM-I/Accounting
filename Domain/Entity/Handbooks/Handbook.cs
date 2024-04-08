namespace Domain.Entity.Handbooks
{
    public abstract class Handbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public Handbook Parent { get; set; }

    }
}
