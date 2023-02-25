using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TransactionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.TransactionService
{
    public interface ITransactionService
    {
        Task<ResultModel> payByCashForAddendum(string token, TransactionRequestFirstModel model);
        Task<ResultModel> payByCashForPayment(string token, TransactionModel model);
    }
}
