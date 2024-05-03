using Application.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers
{
    public class DocumentController : IDocumentController
    {
        private readonly IDbContext _context;
        private readonly IAccumulationRegisterController _accumulationController;
        public DocumentController(IDbContext context, IAccumulationRegisterController accumulationController)
        {
            _context = context;
            _accumulationController = accumulationController;
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

        public async Task<Guid> AddOrUpdateAsync<T>(T document, bool saveChanges = true) where T : Document
        {
            if (string.IsNullOrWhiteSpace(document.Number))
            {
                var data = _context.GetPropertyData<T>();
                document.Number = GetNextNumber(data);
            }

            _context.Update(document);

            if (saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

            return document.Id;
        }

        public async Task RemoveAsync<T>(Guid id, bool saveChanges = true) where T : Document
        {
            var data = _context.GetPropertyData<T>();

            var handbook = await data.FirstOrDefaultAsync(x => x.Id == id);

            if (handbook == null)
                return;

            data.Remove(handbook);
            await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task<ConductedResult> ConductedDoumentAsync<T>(T document) where T : Document
        {
            var result = new ConductedResult();
            var moves = document.GetAccumulationMove();

            Dictionary<Type, IEnumerable<IAccumulationRegister>> oldMoves = new();
            if(document.Id != Guid.Empty)
            {
                Func<IAccumulationRegister, bool> kva = k => k.DocumentId == document.Id;
                foreach(var move in moves)
                {
                    var oldMove = (IEnumerable<IAccumulationRegister>)_accumulationController.GetType().GetMethod("GetListData").MakeGenericMethod(move.Key).Invoke(_accumulationController, [kva]);

                    if(oldMove != null && oldMove.Count() != 0)
                        oldMoves.Add(move.Key, oldMove);                       
                }

            }

            foreach (var move in moves)
            {
                string type = move.Key.Name;

                switch (type) 
                {
                    case nameof(Leftover):
                        {
                            HashSet<Guid> nomenclatures = new();
                            HashSet<Guid> warehouses = new();

                            foreach (Leftover value in move.Value)
                            {
                                if(value.TypeMove == TypeAccumulationRegisterMove.OUTCOMING)
                                {
                                    nomenclatures.Add(value.Nomenclature.Id);
                                    warehouses.Add(value.Warehouse.Id);
                                }
                            }

                            if (nomenclatures.Count == 0)
                                break;

                            var list = _accumulationController.GetListData<Leftover>(w => nomenclatures.Contains(w.NomenclatureId)
                                                                                          && warehouses.Contains(w.WarehouseId));

                            if (list.Count == 0)
                            {
                                foreach (Leftover value in move.Value)
                                    result.Messages.Add($"Не вистачає {value.Value} {value.Nomenclature.BaseUnit.Name} залишків {value.Nomenclature.Name} на {value.Warehouse.Name}");

                                break;
                            }

                            var leftovers = _accumulationController.GetLeftoverList(list,
                                g => new { g.Nomenclature, g.Warehouse },
                                s => new {
                                    id = string.Join("", s.Key.Nomenclature.Id, s.Key.Warehouse.Id),
                                    value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Value : selector.Value * -1)
                                });

                            if (leftovers.Count == 0)
                            {
                                foreach (Leftover value in move.Value)
                                    result.Messages.Add($"Не вистачає {value.Value} {value.Nomenclature.BaseUnit.Name} залишків {value.Nomenclature.Name} на {value.Warehouse.Name}");
                            }
                            else
                            {
                                Dictionary<string, double> oldMove = new();

                                if(oldMoves.TryGetValue(move.Key, out IEnumerable<IAccumulationRegister> res))
                                {
                                    if(res != null)
                                    {
                                        foreach (Leftover item in res)
                                        {
                                            oldMove.Add(string.Join("", item.NomenclatureId, item.WarehouseId), item.Value);
                                        }
                                    }
                                }

                                foreach (Leftover item in move.Value)
                                {
                                    var id = string.Join("", item.Nomenclature.Id, item.Warehouse.Id);
                                    var data = leftovers.FirstOrDefault(x => x.id == id);
                                    double leftover = 0;
                                    double prevValue = 0;

                                    if (data != null)
                                        leftover = data.value;

                                    oldMove.TryGetValue(id, out prevValue);

                                    if (leftover + prevValue - item.Value <= 0)
                                        result.Messages.Add($"Не вистачає {Math.Abs(leftover + prevValue - item.Value)} {item.Nomenclature.BaseUnit.Name} залишків {item.Nomenclature.Name} на {item.Warehouse.Name}");
                                }
                            }

                            break;
                        }
                    default:
                        break;
                }
            }

            if (result.IsSuccess)
            {
                foreach (var item in oldMoves)
                    await _accumulationController.RemoveRangeAsync(item.Value);

                document.Conducted = true;
                document.Date = DateTime.Now;

                await AddOrUpdateAsync(document);

                foreach (var move in moves)
                {
                    foreach (var item in move.Value)
                    {
                        item.DocumentId = document.Id;
                    }

                    await _accumulationController.AddOrUpdateRangeAsync(move.Value);
                }
            }

            return result;
        }

        public async Task UnConductedDoumentAsync<T>(T document) where T : Document
        {
            var moves = document.GetAccumulationMove();

            Dictionary<Type, IEnumerable<IAccumulationRegister>> oldMoves = new();
            if (document.Id != Guid.Empty)
            {
                Func<IAccumulationRegister, bool> kva = k => k.DocumentId == document.Id;
                foreach (var move in moves)
                {
                    var oldMove = (IEnumerable<IAccumulationRegister>)_accumulationController.GetType().GetMethod("GetListData").MakeGenericMethod(move.Key).Invoke(_accumulationController, [kva]);

                    if (oldMove != null && oldMove.Count() != 0)
                        oldMoves.Add(move.Key, oldMove);
                }
            }

            foreach (var item in oldMoves)
                await _accumulationController.RemoveRangeAsync(item.Value);

            document.Conducted = false;

            await AddOrUpdateAsync(document);
        }

    }
}
