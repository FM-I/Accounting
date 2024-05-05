using BL.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BL.Controllers
{
    public class AccumulationRegisterController : IAccumulationRegisterController
    {
        private readonly IDbContext _context;

        public AccumulationRegisterController(IDbContext context)
        {
            _context = context;
        }

        public List<V> GetLeftoverList<T, G, V>(List<T> leftovers, Func<T, G> groupBy, Func<IGrouping<G, T>, V> select) where T : class, ILeftoverRegister
        {
            return leftovers.GroupBy(groupBy).Select(select).ToList();
        }

        public List<T> GetListData<T>(Func<T, bool>? selectionFunc = default) where T : class, IAccumulationRegister
        {
            IQueryable<T> data = _context.GetPropertyData<T>();

            _context.IncludeVirtualProperty(data);

            IEnumerable<T> result = data.AsNoTracking();
            if(selectionFunc != default) 
                result = data.Where(selectionFunc);

            return result.ToList();
        }

        public async Task AddOrUpdateAsync(IAccumulationRegister data, bool saveChanges = true)
        {
            _context.Update(data);
                           
            if(saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task AddOrUpdateRangeAsync(IEnumerable<IAccumulationRegister> data)
        {
            _context.UpdateRange(data);
            await _context.SaveChangesAsync(new CancellationToken());
        }
        
        public async Task RemoveAsync(IAccumulationRegister data)
        {
            _context.Remove(data);
            await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task RemoveRangeAsync(IEnumerable<IAccumulationRegister> data)
        {
            _context.RemoveRange(data);
            await _context.SaveChangesAsync(new CancellationToken());
        }
    }
}
