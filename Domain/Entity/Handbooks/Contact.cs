
namespace Domain.Entity.Handbooks
{
    public class Contact : IHandbook
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Contact)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
