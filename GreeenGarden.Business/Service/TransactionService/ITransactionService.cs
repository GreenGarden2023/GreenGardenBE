using System;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TransactionModel;

namespace GreeenGarden.Business.Service.TransactionService
{
	public interface ITransactionService
	{
		Task<ResultModel> GetTransactionByOrder(string token, TransactionGetByOrderModel transactionGetByOrderModel);
        Task<ResultModel> GetTransactionByDate(string token, TransactionGetByDateModel transactionGetByDateModel);
		Task<ResultModel> CreateATransaction(string token, TransactionOrderCancelModel transactionOrderCancelModel); 
    }
}

