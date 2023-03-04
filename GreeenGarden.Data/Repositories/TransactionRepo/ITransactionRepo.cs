using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TransactionRepo
{
    public interface ITransactionRepo : IRepository<TblTransaction>
    {
        public Task<TblTransaction> insert(TblTransaction transaction);
        public Task<TblPayment> insertPayment(TblPayment payment);
        public Task<TblAddendum> miniusAddendumtAmount(Guid addendumId, double? miniusMoney);
        public Task<double?> checkRemainMoney(Guid addendumId);
        public Task<TblAddendum?> changeStatusAddendum(Guid addendumId, string status);
    }
}
