namespace Domain.Models
{
    public class DataComplectionResult
    {
        public bool Success { get => Properties.Count == 0; }
        public List<string> Properties { get; set; } = new List<string>();
    }
}
