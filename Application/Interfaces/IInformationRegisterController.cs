
using Domain.Interfaces;

namespace BL.Interfaces
{
    public interface IInformationRegisterController
    {
        List<T>? GetLastDateData<T>(DateTime date = default, Func<T, bool>? selectionFunc = default) where T : class, IPeriodDateRegister;
        List<T> GetListData<T>(int skip = 0, int take = 0, Func<T, bool>? selectionFunc = default) where T : class, IInformationRegister;
        Task AddOrUpdateAsync<T>(T data, bool saveChanges = true) where T : class, IInformationRegister;
        Task AddOrUpdateRangeAsync(IEnumerable<IInformationRegister> data);
        Task DeleteAsync<T>(Func<T, bool> func) where T : class, IInformationRegister;
    }
}
