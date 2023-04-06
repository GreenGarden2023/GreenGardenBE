using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TransactionRepo
{
    public interface ITransactionRepo : IRepository<TblTransaction>
    {
        Task<List<TblTransaction>> GetTransactionByOrder(Guid orderId, string orderType);
        Task<List<TblTransaction>> GetTransactionByDateRange(DateTime rangeStart, DateTime rangeEnd);
    }
}
