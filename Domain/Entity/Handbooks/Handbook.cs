namespace Domain.Entity.Handbooks
{
    public abstract class Handbook
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public string IdParent { get; set; }

    }
}
