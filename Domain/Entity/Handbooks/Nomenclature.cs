
namespace Domain.Entity.Handbooks
{
    public class Nomenclature : IHandbook
    {
        public string Arcticle { get; set; }
        public virtual UnitClasificator BaseUnit { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }
        public Nomenclature Parent { get; set; }

        public bool ChekOccupancy()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Code);
        }

        public IHandbook DeepCopy()
        {
            Nomenclature nomenclature = (Nomenclature)MemberwiseClone();
            nomenclature.Id = Guid.Empty;
            return nomenclature;
        }
    }
}
