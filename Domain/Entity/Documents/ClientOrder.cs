using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class ClientOrder : Document
    {
        public Client Client { get; set; }
        public Organization Organization { get; set; }
        public TypePrice TypePrice { get; set; }
        public Currency Currency { get; set; }
        public virtual ICollection<ClientOrderProduct> Products { get; set; }
    }
}
