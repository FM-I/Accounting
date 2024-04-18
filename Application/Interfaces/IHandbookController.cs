using Domain.Entity.Handbooks;

namespace Application.Interfaces
{
    internal interface IHandbookController
    {
        public List<T> GetHandbooks<T>(int skip = 0, int take = 0) where T : class, IHandbook;
        public T? GetHandbook<T>(Guid id) where T : class, IHandbook;
        public Task<Guid> AddOrUpdateHandbookAsync(IHandbook handbook);
        public Task DeleteHandbook<T>(Guid id) where T : class, IHandbook;
    }
}
