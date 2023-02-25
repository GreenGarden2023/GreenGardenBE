using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.TransactionRepo
{
    public interface ITransactionRepo
    {
        public Task<TblTransaction> insert(TblTransaction transaction);
        public Task<TblPayment> insertPayment(TblPayment payment);
        public Task<TblAddendum> miniusAddendumtAmount(Guid addendumId, double? miniusMoney);
        public Task<double?> checkRemainMoney(Guid addendumId);
        public Task<TblAddendum?> changeStatusAddendum(Guid addendumId, string status);
    }
}
