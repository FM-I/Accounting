﻿using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Application.Controllers
{
    public class DocumentController
    {
        private readonly IDbContext _context;

        public DocumentController(IDbContext context)
        {
            _context = context;
        }

        private DbSet<T> GetPropertyData<T>() where T : Document
        {
            PropertyInfo? property = _context.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(DbSet<T>));

            if (property == null)
                throw new Exception("Property not finded");

            object? propValue = property.GetValue(_context);

            if (propValue == null)
                throw new Exception("Property value is null");

            DbSet<T> data = (DbSet<T>)propValue;

            return data;
        }

        private string GetNextCode<T>(DbSet<T> data) where T : Document
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

        public List<T> GetDocuments<T>(int skip = 0, int take = 0) where T : Document
        {
            if (skip < 0)
                throw new Exception("Invalid parameter value 'skip'");

            if (take < 0)
                throw new Exception("Invalid parameter value 'take'");

            var data = GetPropertyData<T>();

            if (skip == 0 && take == 0)
                return data.ToList();

            return data.AsNoTracking().Skip(skip).Take(take).ToList();
        }

        public T? GetDocument<T>(Guid id) where T : Document
        {
            var data = GetPropertyData<T>();

            return data.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<Guid> AddOrUpdateDocumentAsync<T>(Document handbook, bool saveChanges = true) where T : Document
        {
            if (string.IsNullOrWhiteSpace(handbook.Number))
            {
                var data = GetPropertyData<T>();
                handbook.Number = GetNextCode(data);
            }

            //if (!handbook.ChekOccupancy())
            //    return Guid.Empty;

            _context.Update(handbook);

            if (saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

            return handbook.Id;
        }

        public async Task DeleteDocument<T>(Guid id, bool saveChanges = true) where T : Document
        {
            var data = GetPropertyData<T>();

            var handbook = await data.FirstOrDefaultAsync(x => x.Id == id);

            if (handbook == null)
                return;

            data.Remove(handbook);
            await _context.SaveChangesAsync(new CancellationToken());
        }

    }
}
