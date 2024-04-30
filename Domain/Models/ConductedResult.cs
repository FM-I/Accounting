namespace Domain.Models
{
    public class ConductedResult
    {
        public bool IsSuccess { get => Messages.Count == 0; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
