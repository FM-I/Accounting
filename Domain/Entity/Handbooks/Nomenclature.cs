
namespace Domain.Entity.Handbooks
{
    public class Nomenclature : IHandbook
    {
        public string Arcticle { get; set; }
        public UnitClasificator BaseUnit { get; set; }
        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Code { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Nomenclature Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
