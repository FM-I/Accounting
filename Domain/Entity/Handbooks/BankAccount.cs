namespace Domain.Entity.Handbooks
{
    public class BankAccount : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
    }
}
