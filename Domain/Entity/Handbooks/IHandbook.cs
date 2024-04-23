using Domain.Models;

namespace Domain.Entity.Handbooks
{
    public interface IHandbook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsGroup { get; set; }

        public DataComplectionResult CheckDataComplection()
        {
            var result = new DataComplectionResult();

            if (!string.IsNullOrWhiteSpace(Name))
                result.Properties.Add(nameof(Name));

            if (!string.IsNullOrWhiteSpace(Code))
                result.Properties.Add(nameof(Code));

            return result;
        }
        public IHandbook DeepCopy();

    }
}
