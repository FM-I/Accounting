using Domain.Entity.Documents;
using Domain.Models;

namespace BL.Interfaces
{
    public interface IDocumentController
    {
        List<T> GetDocuments<T>(int skip = 0, int take = 0) where T : Document;
        T? GetDocument<T>(Guid id) where T : Document;
        Task<Guid> AddOrUpdateAsync<T>(T document, bool saveChanges = true) where T : Document;
        Task RemoveAsync<T>(Guid id, bool saveChanges = true) where T : Document;
        Task RemoveRangeAsync<T>(Func<T, bool> where) where T : class;
        Task<ConductedResult> ConductedDoumentAsync<T>(T document) where T : Document;
        Task UnConductedDoumentAsync<T>(T document) where T : Document;
    }
}
