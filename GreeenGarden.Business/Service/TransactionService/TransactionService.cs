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
        /*public async Task<ResultModel> payByCashForAddendum(string token, TransactionRequestFirstModel model)
        {
            var result = new ResultModel();
            try
            {
                var addendum = await _orderRepo.GetAddendum(model.AddendumId);
                var transaction = new TblTransaction()
                {
                    Id = Guid.NewGuid(),
                    OrderId = addendum.OrderId,
                    AddendumId = addendum.Id,
                    Amount = model.NumberMoney,
                    Type = TransactionType.RECEIVED,
                    Status = Status.SUCCESSED,
                    DatetimePaid= DateTime.Now,
                };
                await _transactionRepo.insert(transaction);
                var payment = new TblPayment()
                {
                    Id = Guid.NewGuid(),
                    Status = Status.SUCCESSED,
                    PaymentMethod = PaymentMethod.CASH,
                    TransactionId = transaction.Id,
                };
                await _transactionRepo.insertPayment(payment);
                await _transactionRepo.miniusAddendumtAmount(addendum.Id, model.NumberMoney);

                if (await _transactionRepo.checkRemainMoney(addendum.Id) == 0)
                {
                    await _transactionRepo.changeStatusAddendum(addendum.Id, Status.SUCCESSED);
                }

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

        }*/

        
    }
}
