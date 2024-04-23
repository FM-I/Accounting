namespace Domain.Interfaces
{
    public interface IPeriodDateRegister
    {
        public DateTime Date { get; set; }
        public bool Equals(object? obj);
    }
}
