using Application.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers
{
    public class HandbookController : IHandbookController
    {
        private readonly IDbContext _context;

        public HandbookController(IDbContext context)
        {
            _context = context;
        }

        private string GetNextCode<T>(DbSet<T> data) where T : class, IHandbook
        {
            var sortData = data.OrderByDescending(x => x.Code);
            var element = sortData.FirstOrDefault();

            if (element == null)
                return "000000001";

            int number;

            if(int.TryParse(element.Code, out number))
            {
                string result = "";

                for (short i = 0; i < 9 - number.ToString().ToList().Count; ++i)
                    result += "0";

                result += number + 1;

                return result;
            }

            return string.Empty;
        }

        public List<T> GetHandbooks<T>(int skip = 0, int take = 0) where T : class, IHandbook
        {
            if(skip < 0)
                throw new Exception("Invalid parameter value 'skip'");

            if (take < 0)
                throw new Exception("Invalid parameter value 'take'");

            var data = _context.GetPropertyData<T>();
            
            if(skip == 0 && take == 0)
                return data.ToList();

            return data.AsNoTracking().Skip(skip).Take(take).ToList();
        }

        public T? GetHandbook<T>(Guid id) where T : class, IHandbook
        {
            var data = _context.GetPropertyData<T>();

            return data.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public T? GetHandbook<T>(Func<T,bool> func) where T : class, IHandbook
        {
            var data = _context.GetPropertyData<T>();

            return data.AsNoTracking().FirstOrDefault(func);
        }

        public async Task<Guid> AddOrUpdateAsync<T>(IHandbook handbook, bool saveChanges = true) where T : class, IHandbook
        {
            if (string.IsNullOrWhiteSpace(handbook.Code))
            {
                var data = _context.GetPropertyData<T>();
                handbook.Code = GetNextCode(data);
            }

            _context.Update(handbook);

            if(saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

            return handbook.Id;
        }

        public async Task DeleteAsync<T>(Guid id, bool saveChanges = true)  where T : class, IHandbook 
        {
            var data = _context.GetPropertyData<T>();

            var handbook = await data.FirstOrDefaultAsync(x => x.Id == id);

            if (handbook == null)
                return;

            data.Remove(handbook);
            await _context.SaveChangesAsync(new CancellationToken());
        }

    }
}

