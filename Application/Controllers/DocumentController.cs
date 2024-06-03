using BL.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Controllers
{
    public class DocumentController : IDocumentController
    {
        private readonly IDbContext _context;
        private readonly IAccumulationRegisterController _accumulationController;
        private readonly IHandbookController _handbookController;
        public DocumentController(IDbContext context, IAccumulationRegisterController accumulationController, IHandbookController handbookController)
        {
            _context = context;
            _accumulationController = accumulationController;
            _handbookController = handbookController;
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

        public List<T> GetDocuments<T>(int skip = 0, int take = 0) where T : Document
        {
            if (skip < 0)
                throw new Exception("Invalid parameter value 'skip'");

            if (take < 0)
                throw new Exception("Invalid parameter value 'take'");

            IQueryable<T> data = _context.GetPropertyData<T>();

            data = _context.IncludeVirtualProperty(data);

            if (skip == 0 && take == 0)
                return data.AsNoTracking().ToList();

            return data.Skip(skip).Take(take).AsNoTracking().ToList();
        }

        public T? GetDocument<T>(Guid id) where T : Document
        {
            IQueryable<T> data = _context.GetPropertyData<T>();
            data = _context.IncludeVirtualProperty(data);
            var doc = data.AsNoTracking().FirstOrDefault(x => x.Id == id);

            _context.ChangeTracker.Clear();
            return doc;
        }

        public async Task<Guid> AddOrUpdateAsync<T>(T document, bool saveChanges = true) where T : Document
        {
            _context.ChangeTracker.Clear();

            if (string.IsNullOrWhiteSpace(document.Number))
            {
                var data = _context.GetPropertyData<T>();
                document.Number = GetNextNumber(data);
            }

            _context.ChangeTracker.Clear();
            _context.Update(document);

            if (saveChanges)
                await _context.SaveChangesAsync(new CancellationToken());

            return document.Id;
        }

        public async Task RemoveAsync<T>(Guid id, bool saveChanges = true) where T : Document
        {
            var data = _context.GetPropertyData<T>();

            var document = await data.FirstOrDefaultAsync(x => x.Id == id);

            if (document == null)
                return;

            _context.ChangeTracker.Clear();
            data.Remove(document);
            await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task RemoveRangeAsync<T>(Func<T, bool> where) where T : class
        {
            var data = _context.GetPropertyData<T>();

            var document = data.Where(where);

            if (document == null)
                return;

            _context.ChangeTracker.Clear();
            data.RemoveRange(document);
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
                    case nameof(CashInCashBox):
                        {

                            CashInCashBox? item = move.Value.FirstOrDefault() as CashInCashBox;

                            if(item != null && item.TypeMove == TypeAccumulationRegisterMove.OUTCOMING)
                            {

                                var data = _accumulationController.GetListData<CashInCashBox>(s => s.CashBoxId == item.CashBox.Id && s.CurrencyId == item.Currency.Id);
                                var leftovers = _accumulationController.GetLeftoverList(data, g => g.CashBox.Id, s => new { CashBox = s.Key, Summa = s.Sum(sl => sl.TypeMove == TypeAccumulationRegisterMove.INCOMING ? sl.Summa : -sl.Summa) });
                                
                                decimal summa = 0;
                                
                                if(oldMoves.TryGetValue(move.Key, out var res))
                                {
                                    CashInCashBox? prevData = res.FirstOrDefault() as CashInCashBox;

                                    if (prevData != null)
                                        summa = prevData.Summa;
                                }

                                var leftover = leftovers.FirstOrDefault();

                                if (leftover == null || leftover.Summa + summa - item.Summa < 0)
                                    result.Messages.Add($"В касі {item.CashBox.Name} не достатньо коштів: {item.Summa - (leftover != null ? leftover.Summa + summa : 0)} {item.Currency?.Name}");

                            }

                            break;
                        }
                    case nameof(CashInBankAccount):
                        {

                            CashInBankAccount? item = move.Value.FirstOrDefault() as CashInBankAccount;

                            if (item != null && item.TypeMove == TypeAccumulationRegisterMove.OUTCOMING)
                            {

                                var data = _accumulationController.GetListData<CashInBankAccount>(s => s.BankAccountId == item.BankAccount.Id && s.CurrencyId == item.Currency.Id);
                                var leftovers = _accumulationController.GetLeftoverList(data, g => g.BankAccount.Id, s => new { Summa = s.Sum(sl => sl.TypeMove == TypeAccumulationRegisterMove.INCOMING ? sl.Summa : -sl.Summa) });

                                var leftover = leftovers.FirstOrDefault();

                                decimal summa = 0;

                                if (oldMoves.TryGetValue(move.Key, out var res))
                                {
                                    CashInBankAccount? prevData = res.FirstOrDefault() as CashInBankAccount;

                                    if (prevData != null)
                                        summa = prevData.Summa;
                                }

                                if (leftover == null || leftover.Summa + summa - item.Summa < 0)
                                    result.Messages.Add($"На рахунку {item.BankAccount.Name} не достатньо коштів: {item.Summa - (leftover != null ? leftover.Summa + summa - item.Summa : 0)} {item.Currency?.Name}");

                            }

                            break;
                        }
                    case nameof(Leftover):
                        {
                            HashSet<Guid> nomenclatures = new();
                            HashSet<Guid> warehouses = new();

                            foreach (Leftover value in move.Value)
                            {
                                if(value.TypeMove == TypeAccumulationRegisterMove.OUTCOMING)
                                {
                                    nomenclatures.Add(value.NomenclatureId);
                                    warehouses.Add(value.Warehouse.Id);
                                }
                            }

                            if (nomenclatures.Count == 0)
                                break;

                            var list = _accumulationController.GetListData<Leftover>(w => nomenclatures.Contains(w.NomenclatureId)
                                                                                          && warehouses.Contains(w.WarehouseId));

                            var nomenclatureList = _handbookController.GetHandbooks<Nomenclature>(w => nomenclatures.Contains(w.Id));

                            if (list.Count == 0)
                            {

                                foreach (Leftover value in move.Value)
                                {
                                    var nomenclature = nomenclatureList.FirstOrDefault(w => w.Id == value.NomenclatureId);

                                    if(nomenclature != null)
                                        result.Messages.Add($"Не вистачає {value.Value} {nomenclature.BaseUnit.Name} залишків {nomenclature.Name} на {value.Warehouse.Name}");

                                }

                                break;
                            }

                            var leftovers = _accumulationController.GetLeftoverList(list,
                                g => new { g.NomenclatureId, g.Warehouse.Id },
                                s => new {
                                    id = string.Join("", s.Key.NomenclatureId, s.Key.Id),
                                    value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Value : selector.Value * -1)
                                });

                            if (leftovers.Count == 0)
                            {
                                foreach (Leftover value in move.Value)
                                {
                                    var nomenclature = nomenclatureList.FirstOrDefault(w => w.Id == value.NomenclatureId);

                                    if(nomenclature != null)
                                        result.Messages.Add($"Не вистачає {value.Value} {nomenclature.BaseUnit.Name} залишків {nomenclature.Name} на {value.Warehouse.Name}");

                                }
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
                                    var nomenclature = nomenclatureList.FirstOrDefault(w => w.Id == item.NomenclatureId);
                                    var id = string.Join("", item.NomenclatureId, item.Warehouse.Id);
                                    var data = leftovers.FirstOrDefault(x => x.id == id);
                                    double leftover = 0;
                                    double coefficient = 1;
                                    
                                    oldMove.TryGetValue(id, out leftover);

                                    if(nomenclature.BaseUnit != null && nomenclature.BaseUnit.Coefficient > 0)
                                        coefficient = nomenclature.BaseUnit.Coefficient;

                                    if (data != null)
                                        leftover += data.value;


                                    var value = leftover - (item.Value / coefficient);
                                    if (value < 0)
                                    {

                                        if (leftover < 0)
                                            value = item.Value;

                                        if(nomenclature != null)
                                            result.Messages.Add($"Не вистачає  {Math.Abs(value)} {nomenclature.BaseUnit.Name} залишків {nomenclature.Name} на {item.Warehouse.Name}");
                                    }
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
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in oldMoves)
                            foreach (var value in item.Value)
                                await _accumulationController.RemoveAsync(value);

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

                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        result.Messages.Add("Помилка під час проведння документу: " + e.Message);
                    }
                }
            }

            return result;
        }

        public async Task UnConductedDoumentAsync<T>(T document) where T : Document
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
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
                        foreach (var value in item.Value)
                            await _accumulationController.RemoveAsync(value);

                    document.Conducted = false;

                    await AddOrUpdateAsync(document);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
        }

    }
}
