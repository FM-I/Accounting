using Domain.Entity.Documents;

namespace Application.Interfaces
{
    public interface IDocumentController
    {
        public List<T> GetDocuments<T>(int skip = 0, int take = 0, bool includeVirtualProperty = false) where T : Document;
        public T? GetDocument<T>(Guid id) where T : Document;
        public Task<Guid> AddOrUpdateAsync<T>(Document document, bool saveChanges = true) where T : Document;
        public Task DeleteAsync<T>(Guid id, bool saveChanges = true) where T : Document;
    }
}
