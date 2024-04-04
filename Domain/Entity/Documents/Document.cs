using System.Security.Cryptography.X509Certificates;

namespace Domain.Entity.Documents
{
    public abstract class Document
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public bool Conducted { get; set; }
        public string Number { get; set; }

        public void WriteData() { }

    }
}
