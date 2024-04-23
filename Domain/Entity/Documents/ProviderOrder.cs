using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;

namespace Domain.Entity.Documents
{
    public class ProviderOrder : Document
    {
        public TypePrice TypePrice { get; set; }
        public Currency Currency { get; set; }
        public Warehouse Warehouse { get; set; }
        public virtual ICollection<ProviderOrderProduct> Products { get; set; }

    }
}
