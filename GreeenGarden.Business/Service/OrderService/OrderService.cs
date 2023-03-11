using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.RewardRepo;

namespace GreeenGarden.Business.Service.OrderService
{
	public class OrderService : IOrderService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRentOrderRepo _rentOrderRepo;
        private readonly IRentOrderDetailRepo _rentOrderDetailRepo;
        private readonly ISizeProductItemRepo _sizeProductItemRepo;
        private readonly IRewardRepo _rewardRepo;
        public OrderService(IRewardRepo rewardRepo, IRentOrderRepo rentOrderRepo, IRentOrderDetailRepo rentOrderDetailRepo, ISizeProductItemRepo sizeProductItemRepo)
		{
            _decodeToken = new DecodeToken();
            _rentOrderRepo = rentOrderRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _sizeProductItemRepo = sizeProductItemRepo;
            _rewardRepo = rewardRepo;
        }

        public async Task<ResultModel> CreateRentOrder(string token, RentOrderModel rentOrderModel)
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
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                string userName = _decodeToken.Decode(token, "username");
                double totalAmountPerDay = 0;
                int totalQuantity = 0;
                double numberRentDays = 0;
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;
                numberRentDays = Math.Ceiling((rentOrderModel.EndDateRent - rentOrderModel.StartDateRent).TotalDays);
                if (numberRentDays <1)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Please rent for atleast 1 day.";
                    return result;
                }
                foreach (RentOrderDetailModel item in rentOrderModel.ItemList)
                {
                    TblProductItemDetail itemDetail = await _sizeProductItemRepo.Get(item.ID);
                    if(itemDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Atleast 1 product item is invalid.";
                        return result;
                    }
                    else
                    {
                        totalAmountPerDay = (double)(totalAmountPerDay + (item.Quantity * itemDetail.RentPrice));
                        totalQuantity = totalQuantity + item.Quantity;
                    }
                }

                if (totalQuantity <= 10 && totalAmountPerDay <= 1000000)
                {
                    transportFee = ShipFee.L_10QUAN_L_1MIL * totalAmountPerDay;
                }
                else if(totalQuantity >10 && totalQuantity <= 30 && totalAmountPerDay <= 1000000)
                {
                    transportFee = ShipFee.L_30QUAN_L_1MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 30 && totalAmountPerDay <= 1000000)
                {
                    transportFee = ShipFee.M_30QUAN_L_1MIL * totalAmountPerDay;
                }
                else if (totalQuantity <= 10 && totalAmountPerDay <= 10000000)
                {
                    transportFee = ShipFee.L_10QUAN_L_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 10 && totalQuantity <=30 && totalAmountPerDay <= 10000000)
                {
                    transportFee = ShipFee.L_30QUAN_L_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 30 && totalAmountPerDay <= 10000000)
                {
                    transportFee = ShipFee.M_30QUAN_L_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity <=10 && totalAmountPerDay > 10000000)
                {
                    transportFee = ShipFee.L_10QUAN_M_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity >10 && totalQuantity <= 30 && totalAmountPerDay > 10000000)
                {
                    transportFee = ShipFee.L_30QUAN_M_10MIL * totalAmountPerDay;
                }
                else
                {
                    transportFee = ShipFee.M_30QUAN_M_10MIL * totalAmountPerDay;
                }
                
                discountAmount = (double)(rentOrderModel.RewardPointUsed * 1000);
                totalOrderAmount = (numberRentDays * totalAmountPerDay) + transportFee - discountAmount;

                deposit = totalOrderAmount * 0.2;
                rewardPointGain = (int)Math.Ceiling((totalOrderAmount * 0.01)/1000);
                string userID = _decodeToken.Decode(token, "userid");
                TblRentOrder tblRentOrder = new TblRentOrder
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    TransportFee = transportFee,
                    StartDateRent = rentOrderModel.StartDateRent,
                    EndDateRent = rentOrderModel.EndDateRent,
                    Deposit = deposit,
                    TotalPrice = totalOrderAmount,
                    Status = Status.UNPAID,
                    RemainMoney = totalOrderAmount,
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = rentOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,
                    ReferenceOrderId = rentOrderModel.ReferenceOrderID,
                };
                Guid insertRentOrder = await _rentOrderRepo.Insert(tblRentOrder);
                if(insertRentOrder != Guid.Empty)
                {
                    foreach (RentOrderDetailModel item in rentOrderModel.ItemList)
                    {
                        TblProductItemDetail itemDetail = await _sizeProductItemRepo.Get(item.ID);
                        if (itemDetail == null)
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Atleast 1 product item is invalid.";
                            return result;
                        }
                        else
                        {
                            TblRentOrderDetail tblRentOrderDetail = new TblRentOrderDetail
                            {
                                RentOrderId = tblRentOrder.Id,
                                ProductItemDetailId = item.ID,
                                Quantity = item.Quantity,
                                ProductItemDetailTotalPrice = itemDetail.RentPrice * item.Quantity
                            };
                            await _rentOrderDetailRepo.Insert(tblRentOrderDetail);
                        }
                    }
                    await _rewardRepo.UpdateUserRewardPoint(userName, rewardPointGain, (int)rentOrderModel.RewardPointUsed);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create rent order failed.";
                    return result;
                }
                List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                RentOrderResModel rentOrderResModel = new RentOrderResModel
                {
                    Id = tblRentOrder.Id,
                    TransportFee = tblRentOrder.TransportFee,
                    StartDateRent = tblRentOrder.StartDateRent,
                    EndDateRent = tblRentOrder.EndDateRent,
                    Deposit = tblRentOrder.Deposit,
                    TotalPrice = tblRentOrder.TotalPrice,
                    Status = tblRentOrder.Status,
                    RemainMoney = tblRentOrder.RemainMoney,
                    RewardPointGain = tblRentOrder.RewardPointGain,
                    RewardPointUsed = tblRentOrder.RewardPointUsed,
                    ReferenceOrderId = tblRentOrder.ReferenceOrderId,
                    DiscountAmount = tblRentOrder.DiscountAmount,
                    RentOrderDetailList = rentOrderDetailResModels
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = rentOrderResModel;
                result.Message = "Create rent order successful.";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderId)
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
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(rentOrderId);
                string userID = _decodeToken.Decode(token, "userid");
                if (!tblRentOrder.UserId.Equals(Guid.Parse(userID)))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You can only view your order.";
                    return result;
                }
                else
                {
                    List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderId);
                    RentOrderResModel rentOrderResModel = new RentOrderResModel
                    {
                        Id = tblRentOrder.Id,
                        TransportFee = tblRentOrder.TransportFee,
                        StartDateRent = tblRentOrder.StartDateRent,
                        EndDateRent = tblRentOrder.EndDateRent,
                        Deposit = tblRentOrder.Deposit,
                        TotalPrice = tblRentOrder.TotalPrice,
                        Status = tblRentOrder.Status,
                        RemainMoney = tblRentOrder.RemainMoney,
                        RewardPointGain = tblRentOrder.RewardPointGain,
                        RewardPointUsed = tblRentOrder.RewardPointUsed,
                        ReferenceOrderId = tblRentOrder.ReferenceOrderId,
                        DiscountAmount = tblRentOrder.DiscountAmount,
                        RentOrderDetailList = rentOrderDetailResModels
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = rentOrderResModel;
                    result.Message = "Get rent order detail successful.";
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

        public async Task<ResultModel> GetRentOrders(string token)
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
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                string userID = _decodeToken.Decode(token, "userid");
                List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrders(Guid.Parse(userID));
                List<RentOrderResModel> resList = new List<RentOrderResModel>();
                if (listTblRentOrder.Any())
                {
                    foreach(TblRentOrder order in listTblRentOrder)
                    {
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);
                        RentOrderResModel rentOrderResModel = new RentOrderResModel
                        {
                            Id = order.Id,
                            TransportFee = order.TransportFee,
                            StartDateRent = order.StartDateRent,
                            EndDateRent = order.EndDateRent,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            ReferenceOrderId = order.ReferenceOrderId,
                            DiscountAmount = order.DiscountAmount,
                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        resList.Add(rentOrderResModel);
                    }
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = resList;
                result.Message = "Get rent orders successful.";
                return result;

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

