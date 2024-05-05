﻿using Domain.Entity.Handbooks;

namespace BL.Interfaces
{
    public interface IHandbookController
    {
        public List<T> GetHandbooks<T>(int skip = 0, int take = 0) where T : class, IHandbook;
        public T? GetHandbook<T>(Guid id) where T : class, IHandbook;
        public T? GetHandbook<T>(Func<T, bool> func) where T : class, IHandbook;
        public Task<Guid> AddOrUpdateAsync<T>(T handbook, bool saveChanges = true) where T : class, IHandbook;
        public Task AddOrUpdateRangeAsync<T>(IEnumerable<IHandbook> handbooks) where T : class, IHandbook;
        public Task RemoveAsync<T>(Guid id, bool saveChanges = true) where T : class, IHandbook;
    }
}
