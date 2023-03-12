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
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
using GreeenGarden.Data.Repositories.RentOrderGroupRepo;
using GreeenGarden.Data.Models.PaginationModel;
using EntityFrameworkPaginateCore;

namespace GreeenGarden.Business.Service.OrderService
{
	public class OrderService : IOrderService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRentOrderRepo _rentOrderRepo;
        private readonly IRentOrderGroupRepo _rentOrderGroupRepo;
        private readonly IRentOrderDetailRepo _rentOrderDetailRepo;
        private readonly ISaleOrderRepo _saleOrderRepo;
        private readonly ISaleOrderDetailRepo _saleOrderDetailRepo;
        private readonly ISizeProductItemRepo _sizeProductItemRepo;
        private readonly IRewardRepo _rewardRepo;
        public OrderService(IRentOrderGroupRepo rentOrderGroupRepo,
            ISaleOrderRepo saleOrderRepo,
            ISaleOrderDetailRepo saleOrderDetailRepo,
            IRewardRepo rewardRepo,
            IRentOrderRepo rentOrderRepo,
            IRentOrderDetailRepo rentOrderDetailRepo,
            ISizeProductItemRepo sizeProductItemRepo)
		{
            _decodeToken = new DecodeToken();
            _rentOrderRepo = rentOrderRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _sizeProductItemRepo = sizeProductItemRepo;
            _rewardRepo = rewardRepo;
            _saleOrderDetailRepo = saleOrderDetailRepo;
            _saleOrderRepo = saleOrderRepo;
            _rentOrderGroupRepo = rentOrderGroupRepo;
        }

        public async Task<ResultModel> UpdateRentOrderStatus(string token, Guid rentOrderID, string status)
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
                ResultModel updateResult = await _rentOrderRepo.UpdateRentOrderStatus(rentOrderID, status);
                if(updateResult.IsSuccess == true)
                {
                    TblRentOrder rentOrder = await _rentOrderRepo.Get(rentOrderID);
                    List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderID);
                    RentOrderResModel rentOrderResModel = new RentOrderResModel
                    {
                        Id = rentOrder.Id,
                        TransportFee = rentOrder.TransportFee,
                        StartDateRent = rentOrder.StartDateRent,
                        EndDateRent = rentOrder.EndDateRent,
                        Deposit = rentOrder.Deposit,
                        TotalPrice = rentOrder.TotalPrice,
                        Status = rentOrder.Status,
                        RemainMoney = rentOrder.RemainMoney,
                        RewardPointGain = rentOrder.RewardPointGain,
                        RewardPointUsed = rentOrder.RewardPointUsed,
                        RentOrderGroupID = rentOrder.RentOrderGroupId,
                        DiscountAmount = rentOrder.DiscountAmount,
                        RentOrderDetailList = rentOrderDetailResModels
                    };

                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = rentOrderResModel;
                    result.Message = "Update rent order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update rent order failed.";
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

        public async Task<ResultModel> UpdateSaleOrderStatus(string token, Guid saleOrderID, string status)
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
                ResultModel updateResult = await _saleOrderRepo.UpdateSaleOrderStatus(saleOrderID, status);
                if (updateResult.IsSuccess == true)
                {
                    TblSaleOrder saleOrder = await _saleOrderRepo.Get(saleOrderID);
                    List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    SaleOrderResModel saleOrderResModel = new SaleOrderResModel
                    {
                        Id = saleOrder.Id,
                        TransportFee = saleOrder.TransportFee,
                        CreateDate = (DateTime)saleOrder.CreateDate,
                        Deposit = saleOrder.Deposit,
                        TotalPrice = saleOrder.TotalPrice,
                        Status = saleOrder.Status,
                        RemainMoney = saleOrder.RemainMoney,
                        RewardPointGain = saleOrder.RewardPointGain,
                        RewardPointUsed = saleOrder.RewardPointUsed,
                        DiscountAmount = saleOrder.DiscountAmount,
                        RentOrderDetailList = saleOrderDetailResModels
                    };

                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = saleOrderResModel;
                    result.Message = "Update sale order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update sale order failed.";
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

        public async Task<ResultModel> CreateRentOrder(string token, OrderCreateModel rentOrderModel)
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
                foreach (OrderDetailModel item in rentOrderModel.ItemList)
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
                if (rentOrderModel.RentOrderGroupID == Guid.Empty || rentOrderModel.RentOrderGroupID == null)
                {
                    TblRentOrderGroup tblRentOrderGroup = new TblRentOrderGroup
                    {
                        Id = Guid.NewGuid(),
                        GroupTotalAmount = totalOrderAmount,
                        NumberOfOrders = 1,
                        UserId = Guid.Parse(userID)
                    };
                    _ = await _rentOrderGroupRepo.Insert(tblRentOrderGroup);
                    rentOrderModel.RentOrderGroupID = tblRentOrderGroup.Id;
                }
                else
                {
                    ResultModel resultModel = await _rentOrderGroupRepo.UpdateRentOrderGroup((Guid)rentOrderModel.RentOrderGroupID, totalOrderAmount);
                }
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
                    RentOrderGroupId = rentOrderModel.RentOrderGroupID,
                    RecipientAddress = "" + rentOrderModel.RecipientAddress,
                    RecipientPhone = ""+ rentOrderModel.RecipientPhone,
                    RecipientName = "" +rentOrderModel.RecipientName
                };
                Guid insertRentOrder = await _rentOrderRepo.Insert(tblRentOrder);
                if(insertRentOrder != Guid.Empty)
                {
                    foreach (OrderDetailModel item in rentOrderModel.ItemList)
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
                    RentOrderGroupID = tblRentOrder.RentOrderGroupId,
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

        public async Task<ResultModel> CreateSaleOrder(string token, OrderCreateModel saleOrderModel)
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
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;

                foreach (OrderDetailModel item in saleOrderModel.ItemList)
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
                        totalAmountPerDay = (double) (item.Quantity * itemDetail.SalePrice);
                        totalQuantity = totalQuantity + item.Quantity;
                    }
                }
                if (totalQuantity <= 10 && totalAmountPerDay <= 1000000)
                {
                    transportFee = ShipFee.L_10QUAN_L_1MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 10 && totalQuantity <= 30 && totalAmountPerDay <= 1000000)
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
                else if (totalQuantity > 10 && totalQuantity <= 30 && totalAmountPerDay <= 10000000)
                {
                    transportFee = ShipFee.L_30QUAN_L_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 30 && totalAmountPerDay <= 10000000)
                {
                    transportFee = ShipFee.M_30QUAN_L_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity <= 10 && totalAmountPerDay > 10000000)
                {
                    transportFee = ShipFee.L_10QUAN_M_10MIL * totalAmountPerDay;
                }
                else if (totalQuantity > 10 && totalQuantity <= 30 && totalAmountPerDay > 10000000)
                {
                    transportFee = ShipFee.L_30QUAN_M_10MIL * totalAmountPerDay;
                }
                else
                {
                    transportFee = ShipFee.M_30QUAN_M_10MIL * totalAmountPerDay;
                }
                discountAmount = (double)(saleOrderModel.RewardPointUsed * 1000);
                totalOrderAmount =  totalAmountPerDay + transportFee - discountAmount;

                deposit = totalOrderAmount * 0.2;
                rewardPointGain = (int)Math.Ceiling((totalOrderAmount * 0.01) / 1000);
                string userID = _decodeToken.Decode(token, "userid");
                DateTime createDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                TblSaleOrder tblSaleOrder = new TblSaleOrder
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    TransportFee = transportFee,
                    CreateDate = createDate,
                    Deposit = deposit,
                    TotalPrice = totalOrderAmount,
                    Status = Status.UNPAID,
                    RemainMoney = totalOrderAmount,
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = saleOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,
                    RecipientAddress = "" + saleOrderModel.RecipientAddress,
                    RecipientPhone = "" + saleOrderModel.RecipientPhone,
                    RecipientName = "" + saleOrderModel.RecipientName
                };
                Guid insertSaleOrder = await _saleOrderRepo.Insert(tblSaleOrder);
                if (insertSaleOrder != Guid.Empty)
                {
                    foreach (OrderDetailModel item in saleOrderModel.ItemList)
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
                            TblSaleOrderDetail tblSaleOrderDetail = new TblSaleOrderDetail
                            {
                                SaleOderId = tblSaleOrder.Id,
                                ProductItemDetailId = item.ID,
                                Quantity = item.Quantity,
                                ProductItemDetailTotalPrice = itemDetail.RentPrice * item.Quantity
                            };
                            await _saleOrderDetailRepo.Insert(tblSaleOrderDetail);
                        }
                    }
                    await _rewardRepo.UpdateUserRewardPoint(userName, rewardPointGain, (int)saleOrderModel.RewardPointUsed);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create sale order failed.";
                    return result;
                }
                List<SaleOrderDetailResModel> rentOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(tblSaleOrder.Id);
                SaleOrderResModel saleOrderResModel = new SaleOrderResModel
                {
                    Id = tblSaleOrder.Id,
                    TransportFee = tblSaleOrder.TransportFee,
                    CreateDate = (DateTime)tblSaleOrder.CreateDate,
                    Deposit = tblSaleOrder.Deposit,
                    TotalPrice = tblSaleOrder.TotalPrice,
                    Status = tblSaleOrder.Status,
                    RemainMoney = tblSaleOrder.RemainMoney,
                    RewardPointGain = tblSaleOrder.RewardPointGain,
                    RewardPointUsed = tblSaleOrder.RewardPointUsed,
                    DiscountAmount = tblSaleOrder.DiscountAmount,
                    RentOrderDetailList = rentOrderDetailResModels
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = saleOrderResModel;
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
                        RentOrderGroupID = tblRentOrder.RentOrderGroupId,
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

        public async Task<ResultModel> GetRentOrders(string token, PaginationRequestModel pagingModel)
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
                Page<TblRentOrderGroup> tblRentOrderGroups = await _rentOrderGroupRepo.GetRentOrderGroup(pagingModel, Guid.Parse(userID));
                if (tblRentOrderGroups != null)
                {
                    List<RentOrderGroupModel> listGroup = new List<RentOrderGroupModel>();

                    foreach (TblRentOrderGroup tblRentGroup in tblRentOrderGroups.Results)
                    {
                        List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(tblRentGroup.Id);
                        List<RentOrderResModel> resList = new List<RentOrderResModel>();
                        if (listTblRentOrder.Any())
                        {
                            foreach (TblRentOrder order in listTblRentOrder)
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
                                    RentOrderGroupID = order.RentOrderGroupId,
                                    DiscountAmount = order.DiscountAmount,
                                    RentOrderDetailList = rentOrderDetailResModels
                                };
                                resList.Add(rentOrderResModel);
                            }
                        }
                        RentOrderGroupModel rentOrderGroupModel = new RentOrderGroupModel
                        {
                            NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                            TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount,
                            RentOrderList = resList
                        };
                        listGroup.Add(rentOrderGroupModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(tblRentOrderGroups.PageSize)
                        .CurPage(tblRentOrderGroups.CurrentPage)
                        .RecordCount(tblRentOrderGroups.RecordCount)
                        .PageCount(tblRentOrderGroups.PageCount);

                    RentOrderGroupResModel rentOrderGroupResModel = new RentOrderGroupResModel
                    {
                        Paging = paging,
                        RentOrderGroups = listGroup
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = rentOrderGroupResModel;
                    result.Message = "Get rent orders successful.";
                    return result;
                }
                else
                {
                    result.Message = "List empty.";
                    result.IsSuccess = true;
                    result.Code = 200;
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

        public async Task<ResultModel> GetSaleOrderDetail(string token, Guid saleOrderID)
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
                TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(saleOrderID);
                string userID = _decodeToken.Decode(token, "userid");
                if (!tblSaleOrder.UserId.Equals(Guid.Parse(userID)))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You can only view your order.";
                    return result;
                }
                else
                {

                    List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    SaleOrderResModel saleOrderResModel = new SaleOrderResModel
                    {
                        Id = tblSaleOrder.Id,
                        TransportFee = tblSaleOrder.TransportFee,
                        CreateDate = (DateTime)tblSaleOrder.CreateDate,
                        Deposit = tblSaleOrder.Deposit,
                        TotalPrice = tblSaleOrder.TotalPrice,
                        Status = tblSaleOrder.Status,
                        RemainMoney = tblSaleOrder.RemainMoney,
                        RewardPointGain = tblSaleOrder.RewardPointGain,
                        RewardPointUsed = tblSaleOrder.RewardPointUsed,
                        DiscountAmount = tblSaleOrder.DiscountAmount,
                        RentOrderDetailList = saleOrderDetailResModels
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = saleOrderDetailResModels;
                    result.Message = "Get sale order detail successful.";
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

        public async Task<ResultModel> GetSaleOrders(string token, PaginationRequestModel pagingModel)
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
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                Page<TblSaleOrder> listTblSaleOrder = await _saleOrderRepo.GetSaleOrders(pagingModel, userID);
                List<SaleOrderResModel> resList = new List<SaleOrderResModel>();
                if (listTblSaleOrder != null)
                {
                    foreach (TblSaleOrder order in listTblSaleOrder.Results)
                    {
                        List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(order.Id);
                        SaleOrderResModel saleOrderResModel = new SaleOrderResModel
                        {
                            Id = order.Id,
                            TransportFee = order.TransportFee,
                            CreateDate = (DateTime)order.CreateDate,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            DiscountAmount = order.DiscountAmount,
                            RentOrderDetailList = saleOrderDetailResModels
                        };
                        resList.Add(saleOrderResModel);
                    }

                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblSaleOrder.PageSize)
                        .CurPage(listTblSaleOrder.CurrentPage)
                        .RecordCount(listTblSaleOrder.RecordCount)
                        .PageCount(listTblSaleOrder.PageCount);

                    SaleOrderGetResModel saleOrderGetResModel = new SaleOrderGetResModel
                    {
                        Paging = paging,
                        SaleOrderList = resList
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = saleOrderGetResModel;
                    result.Message = "Get sale orders successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get sale orders failed.";
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

        public async Task<ResultModel> GetAllRentOrders(string token, PaginationRequestModel pagingModel)
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
                Page<TblRentOrderGroup> tblRentOrderGroups = await _rentOrderGroupRepo.GetAllRentOrderGroup(pagingModel);
                if (tblRentOrderGroups != null)
                {
                    List<RentOrderGroupModel> listGroup = new List<RentOrderGroupModel>();

                    foreach (TblRentOrderGroup tblRentGroup in tblRentOrderGroups.Results)
                    {
                        List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(tblRentGroup.Id);
                        List<RentOrderResModel> resList = new List<RentOrderResModel>();
                        if (listTblRentOrder.Any())
                        {
                            foreach (TblRentOrder order in listTblRentOrder)
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
                                    RentOrderGroupID = order.RentOrderGroupId,
                                    DiscountAmount = order.DiscountAmount,
                                    RentOrderDetailList = rentOrderDetailResModels
                                };
                                resList.Add(rentOrderResModel);
                            }
                        }
                        RentOrderGroupModel rentOrderGroupModel = new RentOrderGroupModel
                        {
                            NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                            TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount,
                            RentOrderList = resList
                        };
                        listGroup.Add(rentOrderGroupModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(tblRentOrderGroups.PageSize)
                        .CurPage(tblRentOrderGroups.CurrentPage)
                        .RecordCount(tblRentOrderGroups.RecordCount)
                        .PageCount(tblRentOrderGroups.PageCount);

                    RentOrderGroupResModel rentOrderGroupResModel = new RentOrderGroupResModel
                    {
                        Paging = paging,
                        RentOrderGroups = listGroup
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = rentOrderGroupResModel;
                    result.Message = "Get rent orders successful.";
                    return result;
                }
                else
                {
                    result.Message = "List empty.";
                    result.IsSuccess = true;
                    result.Code = 200;
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

        public async Task<ResultModel> GetAllSaleOrders(string token, PaginationRequestModel pagingModel)
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
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                Page<TblSaleOrder> listTblSaleOrder = await _saleOrderRepo.GetAllSaleOrders(pagingModel);
                List<SaleOrderResModel> resList = new List<SaleOrderResModel>();
                if (listTblSaleOrder != null)
                {
                    foreach (TblSaleOrder order in listTblSaleOrder.Results)
                    {
                        List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(order.Id);
                        SaleOrderResModel saleOrderResModel = new SaleOrderResModel
                        {
                            Id = order.Id,
                            TransportFee = order.TransportFee,
                            CreateDate = (DateTime)order.CreateDate,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            DiscountAmount = order.DiscountAmount,
                            RentOrderDetailList = saleOrderDetailResModels
                        };
                        resList.Add(saleOrderResModel);
                    }

                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblSaleOrder.PageSize)
                        .CurPage(listTblSaleOrder.CurrentPage)
                        .RecordCount(listTblSaleOrder.RecordCount)
                        .PageCount(listTblSaleOrder.PageCount);

                    SaleOrderGetResModel saleOrderGetResModel = new SaleOrderGetResModel
                    {
                        Paging = paging,
                        SaleOrderList = resList
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = saleOrderGetResModel;
                    result.Message = "Get sale orders successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get sale orders failed.";
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
