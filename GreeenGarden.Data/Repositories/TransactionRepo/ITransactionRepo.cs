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
        public Task<TblPayment> miniusPaymentAmount(Guid paymentId, double? miniusMoney);
        public Task<TblAddendum> miniusAddendumtAmount(Guid addendumId, double? miniusMoney);
    }
}
