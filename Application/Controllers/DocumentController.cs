using Application.Interfaces;
using Domain.Entity.Documents;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers
{
    public class DocumentController : IDocumentController
    {
        private readonly IDbContext _context;

        public DocumentController(IDbContext context)
        {
            _context = context;
        }

        private string GetNextNumber<T>(DbSet<T> data) where T : Document
        {
            var sortData = data.OrderByDescending(x => x.Number);
            var element = sortData.FirstOrDefault();

            if (element == null)
                return "000000001";

            int number;

            if (int.TryParse(element.Number, out number))
            {
                string result = "";

                for (short i = 0; i < 9 - number.ToString().ToList().Count; ++i)
                    result += "0";

                result += number + 1;

                return result;
            }

            return string.Empty;
        }

        public List<T> GetDocuments<T>(int skip = 0, int take = 0, bool includeVirtualProperty = false) where T : Document
        {
            if (skip < 0)
                throw new Exception("Invalid parameter value 'skip'");

            if (take < 0)
                throw new Exception("Invalid parameter value 'take'");

            IQueryable<T> data = _context.GetPropertyData<T>();

            if (includeVirtualProperty)
            {
                var virtualProperty = typeof(T).GetProperties().Where(p => p.GetGetMethod().IsVirtual);

                foreach (var item in virtualProperty)
                {
                    data = data.Include(item.Name);
                }
            }

            if (skip == 0 && take == 0)
                return data.ToList();

            return data.AsNoTracking().Skip(skip).Take(take).ToList();
        }

        public T? GetDocument<T>(Guid id) where T : Document
        {
            var data = _context.GetPropertyData<T>();

            return data.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<Guid> AddOrUpdateAsync<T>(Document handbook, bool saveChanges = true) where T : Document
        {
            if (string.IsNullOrWhiteSpace(handbook.Number))
            {
                var data = _context.GetPropertyData<T>();
                handbook.Number = GetNextNumber(data);
            }

            _context.Update(handbook);

            if (saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

            return handbook.Id;
        }

        public async Task DeleteAsync<T>(Guid id, bool saveChanges = true) where T : Document
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
