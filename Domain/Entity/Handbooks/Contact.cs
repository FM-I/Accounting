
namespace Domain.Entity.Handbooks
{
    public class Contact : IHandbook
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Code { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
