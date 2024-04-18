
namespace Domain.Entity.Handbooks
{
    public class User : IHandbook
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public bool ChekOccupancy()
        {
            return !string.IsNullOrWhiteSpace(Name) 
                && !string.IsNullOrWhiteSpace(Code)
                && !string.IsNullOrWhiteSpace(Login)
                && !string.IsNullOrWhiteSpace(Password);
        }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (User)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
