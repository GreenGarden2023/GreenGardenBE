using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TransactionModel;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Data.Repositories.TransactionRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.TransactionService
{
    public class TransactionService : ITransactionService
    {
        //private readonly IMapper _mapper;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly DecodeToken _decodeToken;
        public TransactionService(/*IMapper mapper,*/ ITransactionRepo transactionRepo, IOrderRepo orderRepo)
        {
            //_mapper = mapper;
            _transactionRepo = transactionRepo;
            _decodeToken = new DecodeToken();
            _orderRepo = orderRepo;
        }
        public async Task<ResultModel> payByCashForAddendum(string token, TransactionRequestFirstModel model)
        {
            var result = new ResultModel();
            try
            {
                var addendum = await _orderRepo.GetAddendum(model.AddendumId);
                var payment = new TblPayment()
                {
                    Id = Guid.NewGuid(),
                    PaymentMethod = model.PaymentMethod,
                    OrderId = addendum.OrderId,
                    AddendumId = addendum.Id,
                    Amount = model.Amount,
                    Type = model.Type,
                    Status = Status.UNPAID,
                };
                await _transactionRepo.insertPayment(payment);

                var transaction = new TblTransaction()
                {
                    Id = Guid.NewGuid(),
                    Amount = model.NumberMoney,
                    PaymentId = payment.Id,
                    DatetimePay = DateTime.Now,
                    Status = Status.SUCCESSED,                    
                };
                await _transactionRepo.insert(transaction);
                await _transactionRepo.miniusAddendumtAmount(addendum.Id, model.NumberMoney);


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = model;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }

        public async Task<ResultModel> payByCashForPayment(string token, TransactionModel model)
        {
            var result = new ResultModel();
            try
            {

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }
    }
}
