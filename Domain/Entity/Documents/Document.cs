using Domain.Entity.Handbooks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Documents
{
    public abstract class Document
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool Conducted { get; set; }
        [MaxLength(9)]
        public string Number { get; set; }
        public Client Client { get; set; }
        public Organization Organization { get; set; }

        public void WriteData() { }

    }
}
