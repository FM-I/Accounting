﻿using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers
{
    public class InformationRegisterController
    {
        private readonly IDbContext _context;

        public InformationRegisterController(IDbContext context)
        {
            _context = context;
        }

        public List<T>? GetLastDateData<T>(DateTime date = default, Func<T, bool>? selectionFunc = default) where T : class, IPeriodDateRegister
        {
            IQueryable<T> data = _context.GetPropertyData<T>();

            if (date == default)
                date = DateTime.Now;

            IEnumerable<T> result = data.AsNoTracking();

            if (selectionFunc != null)
                result = result.Where(selectionFunc);

            result = result.OrderByDescending(x => x.Date).Where(x => x.Date <= date);

            var listData = new List<T>();
            var dict = new Dictionary<T, bool>();

            foreach(var item in result)
            {
                if (!dict.TryGetValue(item, out bool value))
                {
                    dict.Add(item, true);
                    listData.Add(item);
                }
            }

            return listData;
        }

        public IEnumerable<T> GetListData<T>(int skip = 0, int take = 0, Func<T, bool>? selectionFunc = default) where T : class, IInformationRegister
        {
            IQueryable<T> data = _context.GetPropertyData<T>();

            var virtualProperty = typeof(T).GetProperties().Where(p => p.GetGetMethod().IsVirtual && p.PropertyType != typeof(DateTime));

            foreach (var item in virtualProperty)
                data = data.Include(item.Name);

            IEnumerable<T> result = data.AsNoTracking();

            if (selectionFunc != null)
                result = result.Where(selectionFunc);
            
            if (skip > 0)
                result = result.Skip(skip);

            if (take > 0)
                result = result.Take(take);
                
            return result;
        }

        public async Task AddOrUpdateAsync<T>(IInformationRegister data, bool saveChanges = true) where T : class, IInformationRegister
        {
            _context.Update(data);

            if (saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

        }

        public async Task AddOrUpdateRangeAsync(IEnumerable<IInformationRegister> data)
        {
            _context.UpdateRange(data);
            await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task DeleteAsync<T>(Func<T, bool> func) where T : class, IInformationRegister
        {
            var dataProp = _context.GetPropertyData<T>();

            var value = dataProp.Where(func);

            if (value == null)
                return;

            dataProp.RemoveRange(value);
            await _context.SaveChangesAsync(new CancellationToken());
        }
    }
}