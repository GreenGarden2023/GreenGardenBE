using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TransactionModel;
using Newtonsoft.Json.Linq;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.TransactionRepo;

namespace GreeenGarden.Business.Service.TransactionService
{
	public class TransactionService : ITransactionService
	{
        private readonly DecodeToken _decodeToken;
        private readonly ITransactionRepo _transactionRepo;
        public TransactionService(ITransactionRepo transactionRepo)
        {
            _decodeToken = new DecodeToken();
            _transactionRepo = transactionRepo;
        }

        public async Task<ResultModel> GetTransactionByDate(string token, TransactionGetByDateModel transactionGetByDateModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                List<TblTransaction> tblTransactions = await _transactionRepo.GetTransactionByDateRange(transactionGetByDateModel.StartDate, transactionGetByDateModel.EndDate);
                if (tblTransactions.Any())
                {
                    List<TransactionResModel> resList = new List<TransactionResModel>();
                    foreach (TblTransaction transaction in tblTransactions)
                    {
                        PaymentType paymentType = new PaymentType
                        {
                            Id = transaction.PaymentId,
                            PaymentName = transaction.PaymentId.Equals(PaymentMethod.MOMO) ? "MoMo" : "Cash"
                        };
                        Guid orderId = Guid.Empty; 
                        if ( transaction.RentOrderId != null)
                        {
                            orderId = (Guid)transaction.RentOrderId;
                        }
                        if (transaction.SaleOrderId != null)
                        {
                            orderId = (Guid)transaction.SaleOrderId;
                        }
                        if (transaction.ServiceOrderId != null)
                        {
                            orderId = (Guid)transaction.ServiceOrderId;
                        }
                        TransactionResModel transactionResModel = new TransactionResModel
                        {
                            Id = transaction.Id,
                            OrderID = orderId,
                            Amount = (double)transaction.Amount,
                            PaidDate = (DateTime)transaction.DatetimePaid,
                            Type = transaction.Type,
                            Status = transaction.Status,
                            PaymentType = paymentType
                        };
                        resList.Add(transactionResModel);
                    }
                    resList.Sort((x, y) => y.PaidDate.CompareTo(x.PaidDate));

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = resList;
                    result.Message = "Get list success";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = tblTransactions;
                    result.Message = "List empty";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> GetTransactionByOrder(string token, TransactionGetByOrderModel transactionGetByOrderModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                List<TblTransaction> tblTransactions = await _transactionRepo.GetTransactionByOrder(transactionGetByOrderModel.orderId, transactionGetByOrderModel.orderType);
                if (tblTransactions.Any())
                {
                    List<TransactionResModel> resList = new List<TransactionResModel>();
                    foreach (TblTransaction transaction in tblTransactions)
                    {
                        PaymentType paymentType = new PaymentType
                        {
                            Id = transaction.PaymentId,
                            PaymentName = transaction.PaymentId.Equals(PaymentMethod.MOMO) ? "MoMo" : "Cash"
                        };
                        Guid orderId = Guid.Empty;
                        if (transaction.RentOrderId != null)
                        {
                            orderId = (Guid)transaction.RentOrderId;
                        }
                        if (transaction.SaleOrderId != null)
                        {
                            orderId = (Guid)transaction.SaleOrderId;
                        }
                        if (transaction.ServiceOrderId != null)
                        {
                            orderId = (Guid)transaction.ServiceOrderId;
                        }
                        TransactionResModel transactionResModel = new TransactionResModel
                        {
                            Id = transaction.Id,
                            OrderID = orderId,
                            Amount = (double)transaction.Amount,
                            PaidDate = (DateTime)transaction.DatetimePaid,
                            Type = transaction.Type,
                            Status = transaction.Status,
                            PaymentType = paymentType
                        };
                        resList.Add(transactionResModel);
                    }
                    resList.Sort((x, y) => y.PaidDate.CompareTo(x.PaidDate));

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = resList;
                    result.Message = "Get list success";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = tblTransactions;
                    result.Message = "List empty";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }
    }
}

