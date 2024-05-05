using Domain.Interfaces;

namespace BL.Interfaces
{
    public interface IAccumulationRegisterController
    {
        List<V> GetLeftoverList<T, G, V>(List<T> leftovers, Func<T, G> groupBy, Func<IGrouping<G, T>, V> select) where T : class, ILeftoverRegister;
        List<T> GetListData<T>(Func<T, bool>? selectionFunc = default) where T : class, IAccumulationRegister;
        Task AddOrUpdateAsync(IAccumulationRegister data, bool saveChanges = true);
        Task AddOrUpdateRangeAsync(IEnumerable<IAccumulationRegister> data);
        Task RemoveAsync(IAccumulationRegister data);
        Task RemoveRangeAsync(IEnumerable<IAccumulationRegister> data);
    }
}
