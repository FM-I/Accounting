
namespace Domain.Entity.Handbooks
{
    public class Client : IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public Client Parent { get; set; }

        public IHandbook DeepCopy()
        {
            IHandbook handbook = (Client)MemberwiseClone();
            handbook.Id = Guid.Empty;
            return handbook;
        }
    }
}
