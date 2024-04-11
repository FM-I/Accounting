
namespace Domain.Entity.Handbooks
{
    public class User : IHandbook
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Code { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
