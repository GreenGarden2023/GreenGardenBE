using EntityFrameworkPaginateCore;
using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RentOrderGroupRepo;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
using GreeenGarden.Data.Repositories.ServiceDetailRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.ShippingFeeRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Data.Repositories.TransactionRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using MailKit.Search;
using Org.BouncyCastle.Asn1.X509;
using System.Net.NetworkInformation;
using System.Security.Claims;

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
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        private readonly IProductItemRepo _productItemRepo;
        private readonly IRewardRepo _rewardRepo;
        private readonly ISizeRepo _sizeRepo;
        private readonly ICartService _cartService;
        private readonly IImageRepo _imageRepo;
        private readonly IShippingFeeRepo _shippingFeeRepo;
        private readonly IServiceRepo _serviceRepo;
        private readonly IServiceDetailRepo _serviceDetailRepo;
        private readonly IServiceOrderRepo _serviceOrderRepo;
        private readonly IUserRepo _userRepo;
        private readonly ITransactionRepo _transactionRepo;
        public OrderService(IRentOrderGroupRepo rentOrderGroupRepo,
            IServiceRepo serviceRepo,
            IServiceDetailRepo serviceDetailRepo,
            ISaleOrderRepo saleOrderRepo,
            ISaleOrderDetailRepo saleOrderDetailRepo,
            IRewardRepo rewardRepo,
            IRentOrderRepo rentOrderRepo,
            IRentOrderDetailRepo rentOrderDetailRepo,
            IProductItemDetailRepo sizeProductItemRepo,
            ICartService cartService,
            ISizeRepo sizeRepo,
            IProductItemRepo productItemRepo,
            IImageRepo imageRepo,
            IShippingFeeRepo shippingFeeRepo,
            IServiceOrderRepo serviceOrderRepo,
            ITransactionRepo transactionRepo,
            IUserRepo userRepo)
        {
            _decodeToken = new DecodeToken();
            _serviceRepo = serviceRepo;
            _serviceDetailRepo = serviceDetailRepo;
            _rentOrderRepo = rentOrderRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _productItemDetailRepo = sizeProductItemRepo;
            _rewardRepo = rewardRepo;
            _saleOrderDetailRepo = saleOrderDetailRepo;
            _saleOrderRepo = saleOrderRepo;
            _rentOrderGroupRepo = rentOrderGroupRepo;
            _cartService = cartService;
            _sizeRepo = sizeRepo;
            _productItemRepo = productItemRepo;
            _imageRepo = imageRepo;
            _shippingFeeRepo = shippingFeeRepo;
            _serviceOrderRepo = serviceOrderRepo;
            _userRepo = userRepo;
            _transactionRepo = transactionRepo;
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
                ResultModel updateResult = await _rentOrderRepo.UpdateRentOrderStatus(rentOrderID, status);
                if (updateResult.IsSuccess == true)
                {
                    TblRentOrder rentOrder = await _rentOrderRepo.Get(rentOrderID);
                    List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderID);
                    if (status.Equals(Status.COMPLETED) || status.Equals(Status.CANCEL))
                    {
                        foreach (var i in rentOrderDetailResModels)
                        {
                            var productItemDetail = await _productItemDetailRepo.Get(i.ProductItemDetail.Id);
                            productItemDetail.Quantity += i.ProductItemDetail.Quantity;
                            await _productItemDetailRepo.UpdateProductItemDetail(productItemDetail);
                        }
                    }

                    RentOrderResModel rentOrderResModel = new()
                    {
                        Id = rentOrder.Id,
                        IsTransport = rentOrder.IsTransport,
                        TransportFee = rentOrder.TransportFee,
                        StartRentDate = rentOrder.StartDateRent,
                        EndRentDate = rentOrder.EndDateRent,
                        CreatedBy = rentOrder.CreatedBy,
                        UserId = rentOrder.UserId,
                        Deposit = rentOrder.Deposit,
                        TotalPrice = rentOrder.TotalPrice,
                        Status = rentOrder.Status,
                        RemainMoney = rentOrder.RemainMoney,
                        RewardPointGain = rentOrder.RewardPointGain,
                        RewardPointUsed = rentOrder.RewardPointUsed,
                        RentOrderGroupID = rentOrder.RentOrderGroupId,
                        DiscountAmount = rentOrder.DiscountAmount,
                        RecipientAddress = rentOrder.RecipientAddress,
                        RecipientDistrict = rentOrder.RecipientDistrict,
                        RecipientName = rentOrder.RecipientName,
                        RecipientPhone = rentOrder.RecipientPhone,
                        RentOrderDetailList = rentOrderDetailResModels,
                        Reason = rentOrder.Description,
                        OrderCode = rentOrder.OrderCode

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
                ResultModel updateResult = await _saleOrderRepo.UpdateSaleOrderStatus(saleOrderID, status);
                if (updateResult.IsSuccess == true)
                {
                    TblSaleOrder saleOrder = await _saleOrderRepo.Get(saleOrderID);
                    List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    SaleOrderResModel saleOrderResModel = new()
                    {
                        Id = saleOrder.Id,
                        TransportFee = saleOrder.TransportFee,
                        CreateDate = (DateTime)saleOrder.CreateDate,
                        UserId = saleOrder.UserId,
                        Deposit = saleOrder.Deposit,
                        TotalPrice = saleOrder.TotalPrice,
                        Status = saleOrder.Status,
                        RemainMoney = saleOrder.RemainMoney,
                        RewardPointGain = saleOrder.RewardPointGain,
                        RewardPointUsed = saleOrder.RewardPointUsed,
                        DiscountAmount = saleOrder.DiscountAmount,
                        RecipientAddress = saleOrder.RecipientAddress,
                        RecipientDistrict = saleOrder.RecipientDistrict,
                        RecipientName = saleOrder.RecipientName,
                        RecipientPhone = saleOrder.RecipientPhone,
                        RentOrderDetailList = saleOrderDetailResModels,
                        Reason = saleOrder.Description,
                        OrderCode = saleOrder.OrderCode
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
                string userName = _decodeToken.Decode(token, "username");
                double totalAmountPerDay = 0;
                int totalQuantity = 0;
                double numberRentDays = 0;
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;

                bool shippingIDCheck = false;
                for (int i = 1; i <= 19; i++)
                {
                    if (rentOrderModel.ShippingID == i)
                    {
                        shippingIDCheck = true;
                    }
                }
                if (shippingIDCheck == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "District ID invalid.";
                    return result;
                }

                numberRentDays = Math.Ceiling((rentOrderModel.EndDateRent - rentOrderModel.StartDateRent).TotalDays);
                if (numberRentDays < 1)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Please rent for atleast 1 day.";
                    return result;
                }
                foreach (OrderDetailModel item in rentOrderModel.ItemList)
                {
                    TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);
                    if (itemDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Atleast 1 product item is invalid.";
                        return result;
                    }
                    if (itemDetail.Quantity < item.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Item does not have enough quantity left.";
                        return result;
                    }
                    else
                    {
                        totalAmountPerDay = (double)(totalAmountPerDay + (item.Quantity * itemDetail.RentPrice));
                        totalQuantity += item.Quantity;

                        if (rentOrderModel.IsTransport == true)
                        {
                            TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(rentOrderModel.ShippingID);
                            transportFee = (double)((itemDetail.TransportFee * totalQuantity) + tblShippingFee.FeeAmount);
                        }
                    }
                }



                discountAmount = (double)(rentOrderModel.RewardPointUsed * 1000);
                totalOrderAmount = (numberRentDays * totalAmountPerDay) + transportFee - discountAmount;

                if (totalOrderAmount > 200000)
                {
                    deposit = totalOrderAmount * 0.2;
                }
                rewardPointGain = (int)Math.Ceiling(totalOrderAmount * 0.01 / 1000);
                string userID = _decodeToken.Decode(token, "userid");
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                if (rentOrderModel.RentOrderGroupID == Guid.Empty || rentOrderModel.RentOrderGroupID == null)
                {

                    TblRentOrderGroup tblRentOrderGroup = new()
                    {
                        Id = Guid.NewGuid(),
                        GroupTotalAmount = totalOrderAmount,
                        NumberOfOrders = 1,
                        CreateDate = currentTime,
                        UserId = Guid.Parse(userID)
                    };
                    _ = await _rentOrderGroupRepo.Insert(tblRentOrderGroup);
                    rentOrderModel.RentOrderGroupID = tblRentOrderGroup.Id;
                }
                else
                {
                    List<TblRentOrder> checkPreviousOrder = await _rentOrderRepo.GetRentOrdersByGroup((Guid)rentOrderModel.RentOrderGroupID);
                    foreach (TblRentOrder preOrder in checkPreviousOrder)
                    {
                        if (!preOrder.Status.Equals(Status.PAID))
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Please complete previous order payment to create a new one.";
                            return result;
                        }
                    }
                    ResultModel resultModel = await _rentOrderGroupRepo.UpdateRentOrderGroup((Guid)rentOrderModel.RentOrderGroupID, totalOrderAmount);
                }
                TblRentOrder tblRentOrder = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    CreatedBy = Guid.Parse(userID),
                    IsTransport = rentOrderModel.IsTransport,
                    TransportFee = Math.Ceiling(transportFee),
                    StartDateRent = rentOrderModel.StartDateRent,
                    EndDateRent = rentOrderModel.EndDateRent,
                    Deposit = Math.Ceiling(deposit),
                    TotalPrice = Math.Ceiling(totalOrderAmount),
                    Status = Status.UNPAID,
                    RecipientDistrict = rentOrderModel.ShippingID,
                    RemainMoney = Math.Ceiling(totalOrderAmount),
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = rentOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,
                    RentOrderGroupId = rentOrderModel.RentOrderGroupID,
                    RecipientAddress = "" + rentOrderModel.RecipientAddress,
                    RecipientPhone = "" + rentOrderModel.RecipientPhone,
                    RecipientName = "" + rentOrderModel.RecipientName,
                    CreateDate = currentTime,
                    OrderCode = await GenerateOrderCode()
                };
                Guid insertRentOrder = await _rentOrderRepo.Insert(tblRentOrder);
                if (insertRentOrder != Guid.Empty)
                {

                    foreach (OrderDetailModel item in rentOrderModel.ItemList)
                    {
                        TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);
                        TblProductItem tblProductItem = await _productItemRepo.Get(itemDetail.ProductItemId);
                        TblSize tblSize = await _sizeRepo.Get(itemDetail.SizeId);
                        if (itemDetail == null)
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Atleast 1 product item is invalid.";
                            return result;
                        }
                        else
                        {
                            TblRentOrderDetail tblRentOrderDetail = new()
                            {
                                Id = Guid.NewGuid(),
                                RentOrderId = tblRentOrder.Id,
                                Quantity = item.Quantity,
                                TotalPrice = item.Quantity * itemDetail.RentPrice,
                                RentPricePerUnit = itemDetail.RentPrice,
                                SizeName = tblSize.Name,
                                ProductItemName = tblProductItem.Name,
                                ProductItemDetailId = item.ProductItemDetailID,
                            };
                            _ = await _rentOrderDetailRepo.Insert(tblRentOrderDetail);
                            List<string> itemDetailImages = await _imageRepo.GetImgUrlProductItemDetail(itemDetail.Id);
                            TblImage rentOrderDetailImage = new()
                            {
                                RentOrderDetailId = tblRentOrderDetail.Id,
                                ImageUrl = "" + itemDetailImages[0]
                            };
                            _ = await _imageRepo.Insert(rentOrderDetailImage);
                            _ = await _productItemDetailRepo.UpdateProductItemDetailQuantity(itemDetail.Id, item.Quantity);
                        }
                    }
                    _ = await _rewardRepo.RemoveUserRewardPoint(userName, (int)rentOrderModel.RewardPointUsed);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create rent order failed.";
                    return result;
                }
                List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                RentOrderResModel rentOrderResModel = new()
                {
                    Id = tblRentOrder.Id,
                    UserId = tblRentOrder.UserId,
                    CreatedBy = tblRentOrder.CreatedBy,
                    IsTransport = tblRentOrder.IsTransport,
                    TransportFee = tblRentOrder.TransportFee,
                    StartRentDate = tblRentOrder.StartDateRent,
                    EndRentDate = tblRentOrder.EndDateRent,
                    Deposit = tblRentOrder.Deposit,
                    TotalPrice = tblRentOrder.TotalPrice,
                    Status = tblRentOrder.Status,
                    RemainMoney = tblRentOrder.RemainMoney,
                    RewardPointGain = tblRentOrder.RewardPointGain,
                    RewardPointUsed = tblRentOrder.RewardPointUsed,
                    RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                    DiscountAmount = tblRentOrder.DiscountAmount,
                    RecipientAddress = tblRentOrder.RecipientAddress,
                    RecipientDistrict = tblRentOrder.RecipientDistrict,
                    RecipientName = tblRentOrder.RecipientName,
                    RecipientPhone = tblRentOrder.RecipientPhone,
                    OrderCode = tblRentOrder.OrderCode,
                    CreateDate = tblRentOrder.CreateDate,
                    RentOrderDetailList = rentOrderDetailResModels,

                };
                _ = await _cartService.CleanRentCart(token);
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
                string userName = _decodeToken.Decode(token, "username");
                double totalAmountPerDay = 0;
                int totalQuantity = 0;
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;

                bool shippingIDCheck = false;
                for (int i = 1; i <= 19; i++)
                {
                    if (saleOrderModel.ShippingID == i)
                    {
                        shippingIDCheck = true;
                    }
                }
                if (shippingIDCheck == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "District ID invalid.";
                    return result;
                }


                foreach (OrderDetailModel item in saleOrderModel.ItemList)
                {
                    TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);
                    if (itemDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Atleast 1 product item is invalid.";
                        return result;
                    }
                    if (itemDetail.Quantity < item.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Item does not have enough quantity left.";
                        return result;
                    }
                    else
                    {
                        totalAmountPerDay = (double)(item.Quantity * itemDetail.SalePrice);
                        totalQuantity += item.Quantity;

                        if (saleOrderModel.IsTransport == true)
                        {
                            TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(saleOrderModel.ShippingID);
                            transportFee = (double)((itemDetail.TransportFee * totalQuantity) + tblShippingFee.FeeAmount);
                        }
                    }
                }

                discountAmount = (double)(saleOrderModel.RewardPointUsed * 1000);
                totalOrderAmount = totalAmountPerDay + transportFee - discountAmount;

                if (totalOrderAmount > 500000)
                {
                    deposit = totalOrderAmount * 0.2;
                }
                rewardPointGain = (int)Math.Ceiling(totalOrderAmount * 0.01 / 1000);
                string userID = _decodeToken.Decode(token, "userid");
                DateTime createDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                TblSaleOrder tblSaleOrder = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    IsTransport = saleOrderModel.IsTransport,
                    TransportFee = Math.Ceiling(transportFee),
                    CreateDate = createDate,
                    Deposit = Math.Ceiling(deposit),
                    TotalPrice = Math.Ceiling(totalOrderAmount),
                    Status = Status.UNPAID,
                    RecipientDistrict = saleOrderModel.ShippingID,
                    RemainMoney = Math.Ceiling(totalOrderAmount),
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = saleOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,
                    RecipientAddress = "" + saleOrderModel.RecipientAddress,
                    RecipientPhone = "" + saleOrderModel.RecipientPhone,
                    RecipientName = "" + saleOrderModel.RecipientName,
                    OrderCode = await GenerateOrderCode()
                };
                Guid insertSaleOrder = await _saleOrderRepo.Insert(tblSaleOrder);
                if (insertSaleOrder != Guid.Empty)
                {
                    foreach (OrderDetailModel item in saleOrderModel.ItemList)
                    {
                        TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);
                        TblProductItem tblProductItem = await _productItemRepo.Get(itemDetail.ProductItemId);
                        TblSize tblSize = await _sizeRepo.Get(itemDetail.SizeId);
                        if (itemDetail == null)
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Atleast 1 product item is invalid.";
                            return result;
                        }
                        else
                        {
                            TblSaleOrderDetail tblSaleOrderDetail = new()
                            {
                                Id = Guid.NewGuid(),
                                SaleOderId = tblSaleOrder.Id,
                                ProductItemDetailId = itemDetail.Id,
                                Quantity = item.Quantity,
                                TotalPrice = item.Quantity * itemDetail.SalePrice,
                                SalePricePerUnit = itemDetail.SalePrice,
                                SizeName = tblSize.Name,
                                ProductItemName = tblProductItem.Name

                            };
                            _ = await _saleOrderDetailRepo.Insert(tblSaleOrderDetail);
                            List<string> itemDetailImages = await _imageRepo.GetImgUrlProductItemDetail(itemDetail.Id);
                            TblImage rentOrderDetailImage = new()
                            {
                                SaleOrderDetailId = tblSaleOrderDetail.Id,
                                ImageUrl = "" + itemDetailImages[0]
                            };
                            _ = await _imageRepo.Insert(rentOrderDetailImage);
                            _ = await _productItemDetailRepo.UpdateProductItemDetailQuantity(itemDetail.Id, item.Quantity);
                        }
                    }
                    _ = await _rewardRepo.RemoveUserRewardPoint(userName, (int)saleOrderModel.RewardPointUsed);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create sale order failed.";
                    return result;
                }
                List<SaleOrderDetailResModel> rentOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(tblSaleOrder.Id);
                SaleOrderResModel saleOrderResModel = new()
                {
                    Id = tblSaleOrder.Id,
                    UserId = tblSaleOrder.UserId,
                    IsTransport = tblSaleOrder.IsTransport,
                    TransportFee = tblSaleOrder.TransportFee,
                    CreateDate = (DateTime)tblSaleOrder.CreateDate,
                    Deposit = tblSaleOrder.Deposit,
                    TotalPrice = tblSaleOrder.TotalPrice,
                    Status = tblSaleOrder.Status,
                    RemainMoney = tblSaleOrder.RemainMoney,
                    RewardPointGain = tblSaleOrder.RewardPointGain,
                    RewardPointUsed = tblSaleOrder.RewardPointUsed,
                    DiscountAmount = tblSaleOrder.DiscountAmount,
                    RecipientAddress = tblSaleOrder.RecipientAddress,
                    RecipientDistrict = tblSaleOrder.RecipientDistrict,
                    RecipientName = tblSaleOrder.RecipientName,
                    RecipientPhone = tblSaleOrder.RecipientPhone,
                    OrderCode = tblSaleOrder.OrderCode,
                    RentOrderDetailList = rentOrderDetailResModels
                };
                _ = await _cartService.CleanSaleCart(token);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = saleOrderResModel;
                result.Message = "Create sale order successful.";
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
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(rentOrderId);
                if (tblRentOrder != null)
                {
                    List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderId);
                    RentOrderResModel rentOrderResModel = new()
                    {
                        Id = tblRentOrder.Id,
                        UserId = tblRentOrder.UserId,
                        CreatedBy = tblRentOrder.CreatedBy,
                        IsTransport = tblRentOrder.IsTransport,
                        TransportFee = tblRentOrder.TransportFee,
                        StartRentDate = tblRentOrder.StartDateRent,
                        EndRentDate = tblRentOrder.EndDateRent,
                        Deposit = tblRentOrder.Deposit,
                        TotalPrice = tblRentOrder.TotalPrice,
                        Status = tblRentOrder.Status,
                        RemainMoney = tblRentOrder.RemainMoney,
                        RewardPointGain = tblRentOrder.RewardPointGain,
                        RewardPointUsed = tblRentOrder.RewardPointUsed,
                        RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                        DiscountAmount = tblRentOrder.DiscountAmount,
                        RecipientAddress = tblRentOrder.RecipientAddress,
                        RecipientDistrict = tblRentOrder.RecipientDistrict,
                        RecipientName = tblRentOrder.RecipientName,
                        RecipientPhone = tblRentOrder.RecipientPhone,
                        OrderCode = tblRentOrder.OrderCode,
                        CreateDate = tblRentOrder.CreateDate,
                        Reason = tblRentOrder.Description,
                        RentOrderDetailList = rentOrderDetailResModels
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = rentOrderResModel;
                    result.Message = "Get rent order detail successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Id invalid.";
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
                string userID = _decodeToken.Decode(token, "userid");
                Page<TblRentOrderGroup> tblRentOrderGroups = await _rentOrderGroupRepo.GetRentOrderGroup(pagingModel, Guid.Parse(userID));
                if (tblRentOrderGroups != null)
                {
                    List<RentOrderGroupModel> listGroup = new();

                    foreach (TblRentOrderGroup tblRentGroup in tblRentOrderGroups.Results)
                    {
                        List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(tblRentGroup.Id);
                        List<RentOrderResModel> resList = new();
                        if (listTblRentOrder.Any())
                        {
                            foreach (TblRentOrder order in listTblRentOrder)
                            {
                                List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);
                                RentOrderResModel rentOrderResModel = new()
                                {
                                    Id = order.Id,
                                    UserId = order.UserId,
                                    CreatedBy = order.CreatedBy,
                                    IsTransport = order.IsTransport,
                                    TransportFee = order.TransportFee,
                                    StartRentDate = order.StartDateRent,
                                    EndRentDate = order.EndDateRent,
                                    Deposit = order.Deposit,
                                    TotalPrice = order.TotalPrice,
                                    Status = order.Status,
                                    RemainMoney = order.RemainMoney,
                                    RewardPointGain = order.RewardPointGain,
                                    RewardPointUsed = order.RewardPointUsed,
                                    RentOrderGroupID = order.RentOrderGroupId,
                                    DiscountAmount = order.DiscountAmount,
                                    RecipientAddress = order.RecipientAddress,
                                    RecipientDistrict = order.RecipientDistrict,
                                    RecipientName = order.RecipientName,
                                    RecipientPhone = order.RecipientPhone,
                                    OrderCode = order.OrderCode,
                                    CreateDate = order.CreateDate,
                                    Reason = order.Description,
                                    RentOrderDetailList = rentOrderDetailResModels
                                };
                                resList.Add(rentOrderResModel);
                            }
                        }
                        resList.Sort((x, y) => y.EndRentDate.CompareTo(x.EndRentDate));
                        RentOrderGroupModel rentOrderGroupModel = new()
                        {
                            ID = tblRentGroup.Id,
                            CreateDate = (DateTime)tblRentGroup.CreateDate,
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

                    listGroup.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));


                    RentOrderGroupResModel rentOrderGroupResModel = new()
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
                TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(saleOrderID);
                if (tblSaleOrder != null)
                {
                    List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    SaleOrderResModel saleOrderResModel = new()
                    {
                        Id = tblSaleOrder.Id,
                        UserId = tblSaleOrder.UserId,
                        IsTransport = tblSaleOrder.IsTransport,
                        TransportFee = tblSaleOrder.TransportFee,
                        CreateDate = (DateTime)tblSaleOrder.CreateDate,
                        Deposit = tblSaleOrder.Deposit,
                        TotalPrice = tblSaleOrder.TotalPrice,
                        Status = tblSaleOrder.Status,
                        RemainMoney = tblSaleOrder.RemainMoney,
                        RewardPointGain = tblSaleOrder.RewardPointGain,
                        RewardPointUsed = tblSaleOrder.RewardPointUsed,
                        DiscountAmount = tblSaleOrder.DiscountAmount,
                        RecipientAddress = tblSaleOrder.RecipientAddress,
                        RecipientDistrict = tblSaleOrder.RecipientDistrict,
                        RecipientName = tblSaleOrder.RecipientName,
                        RecipientPhone = tblSaleOrder.RecipientPhone,
                        OrderCode = tblSaleOrder.OrderCode,
                        Reason = tblSaleOrder.Description,
                        
                        RentOrderDetailList = saleOrderDetailResModels
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = saleOrderDetailResModels;
                    result.Message = "Get sale order detail successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Id invalid.";
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
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                Page<TblSaleOrder> listTblSaleOrder = await _saleOrderRepo.GetSaleOrders(pagingModel, userID);
                List<SaleOrderResModel> resList = new();
                if (listTblSaleOrder != null)
                {
                    foreach (TblSaleOrder order in listTblSaleOrder.Results)
                    {
                        List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(order.Id);
                        SaleOrderResModel saleOrderResModel = new()
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            IsTransport = order.IsTransport,
                            TransportFee = order.TransportFee,
                            CreateDate = (DateTime)order.CreateDate,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            RentOrderDetailList = saleOrderDetailResModels
                        };
                        resList.Add(saleOrderResModel);
                    }

                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblSaleOrder.PageSize)
                        .CurPage(listTblSaleOrder.CurrentPage)
                        .RecordCount(listTblSaleOrder.RecordCount)
                        .PageCount(listTblSaleOrder.PageCount);

                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));
                    SaleOrderGetResModel saleOrderGetResModel = new()
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
                Page<TblRentOrderGroup> tblRentOrderGroups = await _rentOrderGroupRepo.GetAllRentOrderGroup(pagingModel);
                if (tblRentOrderGroups != null)
                {
                    List<RentOrderGroupModel> listGroup = new();

                    foreach (TblRentOrderGroup tblRentGroup in tblRentOrderGroups.Results)
                    {
                        List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(tblRentGroup.Id);
                        List<RentOrderResModel> resList = new();
                        if (listTblRentOrder.Any())
                        {
                            foreach (TblRentOrder order in listTblRentOrder)
                            {
                                List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);
                                RentOrderResModel rentOrderResModel = new()
                                {
                                    Id = order.Id,
                                    UserId = order.UserId,
                                    CreatedBy = order.CreatedBy,
                                    CreateDate = order.CreateDate,
                                    IsTransport = order.IsTransport,
                                    TransportFee = order.TransportFee,
                                    StartRentDate = order.StartDateRent,
                                    EndRentDate = order.EndDateRent,
                                    Deposit = order.Deposit,
                                    TotalPrice = order.TotalPrice,
                                    Status = order.Status,
                                    RemainMoney = order.RemainMoney,
                                    RewardPointGain = order.RewardPointGain,
                                    RewardPointUsed = order.RewardPointUsed,
                                    RentOrderGroupID = order.RentOrderGroupId,
                                    DiscountAmount = order.DiscountAmount,
                                    RecipientAddress = order.RecipientAddress,
                                    RecipientDistrict = order.RecipientDistrict,
                                    RecipientName = order.RecipientName,
                                    RecipientPhone = order.RecipientPhone,
                                    OrderCode = order.OrderCode,
                                    Reason = order.Description,

                                    RentOrderDetailList = rentOrderDetailResModels
                                };
                                resList.Add(rentOrderResModel);
                            }
                        }

                        resList.Sort((x, y) => y.EndRentDate.CompareTo(x.EndRentDate));

                        RentOrderGroupModel rentOrderGroupModel = new()
                        {
                            ID = tblRentGroup.Id,
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


                    listGroup.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    RentOrderGroupResModel rentOrderGroupResModel = new()
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
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                Page<TblSaleOrder> listTblSaleOrder = await _saleOrderRepo.GetAllSaleOrders(pagingModel);
                List<SaleOrderResModel> resList = new();
                if (listTblSaleOrder != null)
                {
                    foreach (TblSaleOrder order in listTblSaleOrder.Results)
                    {
                        List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(order.Id);
                        SaleOrderResModel saleOrderResModel = new()
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            IsTransport = order.IsTransport,
                            TransportFee = order.TransportFee,
                            CreateDate = (DateTime)order.CreateDate,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            RentOrderDetailList = saleOrderDetailResModels
                        };
                        resList.Add(saleOrderResModel);
                    }

                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblSaleOrder.PageSize)
                        .CurPage(listTblSaleOrder.CurrentPage)
                        .RecordCount(listTblSaleOrder.RecordCount)
                        .PageCount(listTblSaleOrder.PageCount);


                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    SaleOrderGetResModel saleOrderGetResModel = new()
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

        private async Task<string> GenerateOrderCode()
        {
            string orderCode = "";
            bool dup = true;
            while (dup == true)
            {
                Random random = new();
                orderCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10).Select(s => s[random.Next(s.Length)]).ToArray());
                bool checkRentOrder = await _rentOrderRepo.CheckOrderCode(orderCode);
                bool checkSaleOrder = await _saleOrderRepo.CheckOrderCode(orderCode);
                bool checkServiceOrder = await _serviceOrderRepo.CheckOrderCode(orderCode);
                dup = checkRentOrder != false || checkSaleOrder != false || checkServiceOrder != false;
            }

            return orderCode;
        }

        public async Task<ResultModel> GetRentOrdersByGroup(string token, Guid groupID)
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
                TblRentOrderGroup tblRentOrderGroups = await _rentOrderGroupRepo.Get(groupID);
                List<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(tblRentOrderGroups.Id);
                List<RentOrderResModel> resList = new();
                if (listTblRentOrder.Any())
                {
                    foreach (TblRentOrder order in listTblRentOrder)
                    {
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);
                        RentOrderResModel rentOrderResModel = new()
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            CreatedBy = order.CreatedBy,
                            IsTransport = order.IsTransport,
                            TransportFee = order.TransportFee,
                            StartRentDate = order.StartDateRent,
                            EndRentDate = order.EndDateRent,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            RentOrderGroupID = order.RentOrderGroupId,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            Reason = order.Description,
                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        resList.Add(rentOrderResModel);
                    }
                }
                resList.Sort((x, y) => y.EndRentDate.CompareTo(x.EndRentDate));
                RentOrderGroupModel rentOrderGroupModel = new()
                {
                    ID = tblRentOrderGroups.Id,
                    CreateDate = (DateTime)tblRentOrderGroups.CreateDate,
                    NumberOfOrder = (int)tblRentOrderGroups.NumberOfOrders,
                    TotalGroupAmount = (double)tblRentOrderGroups.GroupTotalAmount,
                    RentOrderList = resList
                };
                result.Message = "Get rent group success";
                result.Data = rentOrderGroupModel;
                result.IsSuccess = true;
                result.Code = 200;
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

        public async Task<ResultModel> CalculateRentOrder(OrderCreateModel rentOrderModel)
        {

            ResultModel result = new();
            try
            {

                double totalAmountPerDay = 0;
                int totalQuantity = 0;
                double numberRentDays = 0;
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;

                bool shippingIDCheck = false;
                for (int i = 1; i <= 19; i++)
                {
                    if (rentOrderModel.ShippingID == i)
                    {
                        shippingIDCheck = true;
                    }
                }
                if (shippingIDCheck == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "District ID invalid.";
                    return result;
                }

                numberRentDays = Math.Ceiling((rentOrderModel.EndDateRent - rentOrderModel.StartDateRent).TotalDays);
                if (numberRentDays < 1)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Please rent for atleast 1 day.";
                    return result;
                }
                foreach (OrderDetailModel item in rentOrderModel.ItemList)
                {
                    TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);
                    if (itemDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Atleast 1 product item is invalid.";
                        return result;
                    }
                    if (itemDetail.Quantity < item.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Item does not have enough quantity left.";
                        return result;
                    }
                    else
                    {
                        totalAmountPerDay = (double)(totalAmountPerDay + (item.Quantity * itemDetail.RentPrice));
                        totalQuantity += item.Quantity;
                        if (rentOrderModel.IsTransport == true)
                        {
                            TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(rentOrderModel.ShippingID);
                            transportFee = (double)((itemDetail.TransportFee * totalQuantity) + tblShippingFee.FeeAmount);
                        }
                    }
                }

                discountAmount = (double)(rentOrderModel.RewardPointUsed * 1000);
                totalOrderAmount = (numberRentDays * totalAmountPerDay) + transportFee - discountAmount;

                if (totalOrderAmount > 200000)
                {
                    deposit = totalOrderAmount * 0.2;
                }
                rewardPointGain = (int)Math.Ceiling(totalOrderAmount * 0.01 / 1000);

                TblRentOrder tblRentOrder = new()
                {
                    TransportFee = Math.Ceiling(transportFee),
                    StartDateRent = rentOrderModel.StartDateRent,
                    EndDateRent = rentOrderModel.EndDateRent,
                    Deposit = Math.Ceiling(deposit),
                    TotalPrice = Math.Ceiling(totalOrderAmount),
                    Status = Status.UNPAID,
                    RemainMoney = Math.Ceiling(totalOrderAmount),
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = rentOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,
                    RentOrderGroupId = rentOrderModel.RentOrderGroupID,
                    RecipientAddress = "" + rentOrderModel.RecipientAddress,
                    RecipientPhone = "" + rentOrderModel.RecipientPhone,
                    RecipientName = "" + rentOrderModel.RecipientName,
                };
                int maxPoint = (int)Math.Floor(((numberRentDays * totalAmountPerDay) + transportFee) / 1000) - 50;
                if (maxPoint < 0)
                {
                    maxPoint = 0;
                }
                OrderCalculateModel orderCalculateModel = new()
                {
                    TransportFee = tblRentOrder.TransportFee,
                    Deposit = tblRentOrder.Deposit,
                    TotalPrice = tblRentOrder.TotalPrice,
                    RemainMoney = tblRentOrder.RemainMoney,
                    MaxPoint = maxPoint,
                    RewardPointGain = tblRentOrder.RewardPointGain,
                    RewardPointUsed = tblRentOrder.RewardPointUsed,
                    DiscountAmount = tblRentOrder.DiscountAmount
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = orderCalculateModel;
                result.Message = "Calculate rent order successful.";
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

        public async Task<ResultModel> CalculateSaleOrder(OrderCreateModel saleOrderModel)
        {

            ResultModel result = new();
            try
            {
                double totalAmountPerDay = 0;
                int totalQuantity = 0;
                double totalOrderAmount = 0;
                double transportFee = 0;
                double deposit = 0;
                int rewardPointGain = 0;
                double discountAmount = 0;

                bool shippingIDCheck = false;
                for (int i = 1; i <= 19; i++)
                {
                    if (saleOrderModel.ShippingID == i)
                    {
                        shippingIDCheck = true;
                    }
                }
                if (shippingIDCheck == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "District ID invalid.";
                    return result;
                }
                foreach (OrderDetailModel item in saleOrderModel.ItemList)
                {
                    TblProductItemDetail itemDetail = await _productItemDetailRepo.Get(item.ProductItemDetailID);

                    if (itemDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Atleast 1 product item is invalid.";
                        return result;
                    }
                    if (itemDetail.Quantity < item.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Item does not have enough quantity left.";
                        return result;
                    }
                    else
                    {
                        totalAmountPerDay = (double)(item.Quantity * itemDetail.SalePrice);
                        totalQuantity += item.Quantity;
                        if (saleOrderModel.IsTransport == true)
                        {
                            TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(saleOrderModel.ShippingID);
                            transportFee = (double)((itemDetail.TransportFee * totalQuantity) + tblShippingFee.FeeAmount);
                        }
                    }
                }

                discountAmount = (double)(saleOrderModel.RewardPointUsed * 1000);
                totalOrderAmount = totalAmountPerDay + transportFee - discountAmount;

                if (totalOrderAmount > 500000)
                {
                    deposit = totalOrderAmount * 0.2;
                }
                rewardPointGain = (int)Math.Ceiling(totalOrderAmount * 0.01 / 1000);
                DateTime createDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                TblSaleOrder tblSaleOrder = new()
                {

                    TransportFee = Math.Ceiling(transportFee),
                    CreateDate = createDate,
                    Deposit = Math.Ceiling(deposit),
                    TotalPrice = Math.Ceiling(totalOrderAmount),
                    Status = Status.UNPAID,
                    RemainMoney = Math.Ceiling(totalOrderAmount),
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = saleOrderModel.RewardPointUsed,
                    DiscountAmount = discountAmount,

                };
                double maxPoint = (int)Math.Floor((totalAmountPerDay + transportFee) / 1000) - 50;
                if (maxPoint < 0)
                {
                    maxPoint = 0;
                }
                OrderCalculateModel orderCalculateModel = new()
                {
                    TransportFee = tblSaleOrder.TransportFee,
                    Deposit = tblSaleOrder.Deposit,
                    TotalPrice = tblSaleOrder.TotalPrice,
                    RemainMoney = tblSaleOrder.RemainMoney,
                    MaxPoint = maxPoint,
                    RewardPointGain = tblSaleOrder.RewardPointGain,
                    RewardPointUsed = tblSaleOrder.RewardPointUsed,
                    DiscountAmount = tblSaleOrder.DiscountAmount
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = orderCalculateModel;
                result.Message = "Calculate sale order successful.";
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

        public async Task<ResultModel> GetARentOrder(string token, Guid rentOrderID)
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
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(rentOrderID);
                if (tblRentOrder != null)
                {

                    List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                    RentOrderResModel rentOrderResModel = new()
                    {
                        Id = tblRentOrder.Id,
                        UserId = tblRentOrder.UserId,
                        CreatedBy = tblRentOrder.CreatedBy,
                        IsTransport = tblRentOrder.IsTransport,
                        TransportFee = tblRentOrder.TransportFee,
                        StartRentDate = tblRentOrder.StartDateRent,
                        EndRentDate = tblRentOrder.EndDateRent,
                        Deposit = tblRentOrder.Deposit,
                        TotalPrice = tblRentOrder.TotalPrice,
                        Status = tblRentOrder.Status,
                        RemainMoney = tblRentOrder.RemainMoney,
                        RewardPointGain = tblRentOrder.RewardPointGain,
                        RewardPointUsed = tblRentOrder.RewardPointUsed,
                        RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                        DiscountAmount = tblRentOrder.DiscountAmount,
                        RecipientAddress = tblRentOrder.RecipientAddress,
                        RecipientDistrict = tblRentOrder.RecipientDistrict,
                        RecipientName = tblRentOrder.RecipientName,
                        RecipientPhone = tblRentOrder.RecipientPhone,
                        OrderCode = tblRentOrder.OrderCode,
                        CreateDate = tblRentOrder.CreateDate,
                        Reason = tblRentOrder.Description,
                        RentOrderDetailList = rentOrderDetailResModels
                    };
                    List<ProductItemDetailResModel> productItems = new();
                    foreach (RentOrderDetailResModel model in rentOrderDetailResModels)
                    {
                        TblProductItemDetail detail = await _productItemDetailRepo.Get(model.ProductItemDetail.Id);
                        TblSize? sizeGet = await _sizeRepo.Get(detail.SizeId);
                        List<string> imgGet = await _imageRepo.GetImgUrlProductItemDetail(detail.Id);
                        SizeResModel size = new()
                        {
                            Id = sizeGet.Id,
                            SizeName = sizeGet.Name
                        };
                        ProductItemDetailResModel sizeProd = new()
                        {
                            Id = detail.Id,
                            Size = size,
                            RentPrice = detail.RentPrice,
                            SalePrice = detail.SalePrice,
                            Quantity = detail.Quantity,
                            Status = detail.Status,
                            ImagesURL = imgGet
                        };
                        productItems.Add(sizeProd);
                    }
                    GetARentOrderRes getARentOrderRes = new()
                    {
                        RentOrder = rentOrderResModel,
                        ProductItemDetailList = productItems
                    };

                    result.Message = "Get rent order success";
                    result.Data = getARentOrderRes;
                    result.IsSuccess = true;
                    result.Code = 200;
                    return result;
                }
                else
                {
                    result.Message = "Get rent order failed.";
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

        public async Task<ResultModel> CreateServiceOrder(string token, ServiceOrderCreateModel serviceOrderCreateModel)
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
                string userId = _decodeToken.Decode(token, "userid");
                TblService tblService = await _serviceRepo.Get(serviceOrderCreateModel.ServiceId);
                if (tblService == null)
                {
                    result.Message = "Service Id invalid";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }
                List<ServiceDetailResModel> serviceDetailResModels = await _serviceDetailRepo.GetServiceDetailByServiceID(tblService.Id);

                double serviceDetailTotal = 0;
                foreach (ServiceDetailResModel serviceDetail in serviceDetailResModels)
                {
                    serviceDetailTotal += serviceDetail.ServicePrice ?? 0;
                }

                double finalTotal = (double)(serviceDetailTotal - (tblService.RewardPointUsed * 1000) + tblService.TransportFee);
                int rewardPointGain = (int)Math.Ceiling(finalTotal * 0.01 / 1000);
                double deposit = Math.Ceiling(finalTotal / 2);

                DateTime createDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                TblServiceOrder tblServiceOrder = new()
                {
                    Id = Guid.NewGuid(),
                    OrderCode = await GenerateOrderCode(),
                    CreateDate = createDate,
                    ServiceStartDate = tblService.StartDate,
                    ServiceEndDate = tblService.EndDate,
                    Deposit = deposit,
                    TotalPrice = finalTotal,
                    RemainAmount = finalTotal,
                    Status = ServiceOrderStatus.UNPAID,
                    RewardPointGain = rewardPointGain,
                    RewardPointUsed = tblService.RewardPointUsed,
                    DiscountAmount = tblService.RewardPointUsed * 1000,
                    TechnicianId = (Guid)tblService.TechnicianId,
                    ServiceId = serviceOrderCreateModel.ServiceId,
                    UserId = tblService.UserId,
                    
                    TransportFee = tblService.TransportFee,
                };
                _ = await _rewardRepo.RemoveUserRewardPointByUserID(tblService.UserId, (int)tblService.RewardPointUsed);
                Guid insert = await _serviceOrderRepo.Insert(tblServiceOrder);
                _ = await _serviceRepo.ChangeServiceStatus(tblServiceOrder.ServiceId, ServiceStatus.CONFIRMED);
                if (insert != Guid.Empty)
                {
                    TblServiceOrder tblServiceOrderGet = await _serviceOrderRepo.Get(tblServiceOrder.Id);
                    ServiceOrderCreateResModel resModel = new()
                    {
                        Id = tblServiceOrderGet.Id,
                        CreateDate = tblServiceOrderGet.CreateDate,
                        ServiceStartDate = (DateTime)tblServiceOrderGet.ServiceStartDate,
                        ServiceEndDate = (DateTime)tblServiceOrderGet.ServiceEndDate,
                        Deposit = (double)tblServiceOrderGet.Deposit,
                        TotalPrice = (double)tblServiceOrderGet.TotalPrice,
                        DiscountAmount = (double)tblServiceOrderGet.DiscountAmount,
                        RemainAmount = (double)tblServiceOrderGet.RemainAmount,
                        RewardPointGain = (int)tblServiceOrderGet.RewardPointGain,
                        RewardPointUsed = (int)tblServiceOrderGet.RewardPointUsed,
                        TechnicianID = tblServiceOrderGet.TechnicianId,
                        ServiceID = tblServiceOrderGet.ServiceId,
                        UserID = tblServiceOrderGet.UserId,
                        TransportFee = (double)tblServiceOrderGet.TransportFee,
                        
                        Status = tblServiceOrderGet.Status
                    };
                    result.Message = "Create service order success.";
                    result.Data = resModel;
                    result.IsSuccess = true;
                    result.Code = 200;
                    return result;
                }
                else
                {
                    result.Message = "Create service order failed.";
                    result.IsSuccess = false;
                    result.Code = 400;
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

        public async Task<ResultModel> GetServiceOrders(string token, PaginationRequestModel pagingModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.GetServiceOrders(pagingModel, userID);
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        ServiceResModel serviceResModel = new()
                        {
                            ID = resService.Id,
                            ServiceCode = resService.ServiceCode,
                            UserId = resService.UserId,
                            Rules = resService.Rules,
                            CreateDate = resService.CreateDate ?? DateTime.MinValue,
                            StartDate = resService.StartDate,
                            EndDate = resService.EndDate,
                            Name = resService.Name,
                            Phone = resService.Phone,
                            Email = resService.Email,
                            Address = resService.Address,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        ServiceOrderGetResModel serviceOrderGetResModel = new()
                        {
                            Id = order.Id,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            ServiceStartDate = (DateTime)order.ServiceStartDate,
                            ServiceEndDate = (DateTime)order.ServiceEndDate,
                            Deposit = (double)order.Deposit,
                            TotalPrice = (double)order.TotalPrice,
                            DiscountAmount = (double)order.DiscountAmount,
                            RemainAmount = (double)order.RemainAmount,
                            RewardPointGain = (int)order.RewardPointGain,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            Status = order.Status,
                            Reason = order.Description,
                            Service = serviceResModel
                        };
                        resList.Add(serviceOrderGetResModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblServiceOrders.PageSize)
                        .CurPage(listTblServiceOrders.CurrentPage)
                        .RecordCount(listTblServiceOrders.RecordCount)
                        .PageCount(listTblServiceOrders.PageCount);

                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    ServiceOrderListRes serviceOrderListRes = new()
                    {
                        Paging = paging,
                        ServiceOrderList = resList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderListRes;
                    result.Message = "Get service orders success.";
                    return result;

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get service orders failed.";
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

        public async Task<ResultModel> GetAllServiceOrders(string token, PaginationRequestModel pagingModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.GetAllServiceOrders(pagingModel);
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        ServiceResModel serviceResModel = new()
                        {
                            ID = resService.Id,
                            ServiceCode = resService.ServiceCode,
                            UserId = resService.UserId,
                            Rules = resService.Rules,
                            CreateDate = resService.CreateDate ?? DateTime.MinValue,
                            StartDate = resService.StartDate,
                            EndDate = resService.EndDate,
                            Name = resService.Name,
                            Phone = resService.Phone,
                            Email = resService.Email,
                            Address = resService.Address,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        ServiceOrderGetResModel serviceOrderGetResModel = new()
                        {
                            Id = order.Id,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            ServiceStartDate = (DateTime)order.ServiceStartDate,
                            ServiceEndDate = (DateTime)order.ServiceEndDate,
                            Deposit = (double)order.Deposit,
                            TotalPrice = (double)order.TotalPrice,
                            DiscountAmount = (double)order.DiscountAmount,
                            RemainAmount = (double)order.RemainAmount,
                            RewardPointGain = (int)order.RewardPointGain,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            Status = order.Status,
                            Reason = order.Description,
                            Service = serviceResModel
                        };
                        resList.Add(serviceOrderGetResModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblServiceOrders.PageSize)
                        .CurPage(listTblServiceOrders.CurrentPage)
                        .RecordCount(listTblServiceOrders.RecordCount)
                        .PageCount(listTblServiceOrders.PageCount);

                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    ServiceOrderListRes serviceOrderListRes = new()
                    {
                        Paging = paging,
                        ServiceOrderList = resList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderListRes;
                    result.Message = "Get service orders success.";
                    return result;

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get service orders failed.";
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

        public async Task<ResultModel> GetServiceOrderByTechnician(string token, PaginationRequestModel pagingModel, Guid techinicianID)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.GetServiceOrderByTechnician(pagingModel, techinicianID);
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        ServiceResModel serviceResModel = new()
                        {
                            ID = resService.Id,
                            ServiceCode = resService.ServiceCode,
                            UserId = resService.UserId,
                            Rules = resService.Rules,
                            CreateDate = resService.CreateDate ?? DateTime.MinValue,
                            StartDate = resService.StartDate,
                            EndDate = resService.EndDate,
                            Name = resService.Name,
                            Phone = resService.Phone,
                            Email = resService.Email,
                            Address = resService.Address,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        ServiceOrderGetResModel serviceOrderGetResModel = new()
                        {
                            Id = order.Id,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            ServiceStartDate = (DateTime)order.ServiceStartDate,
                            ServiceEndDate = (DateTime)order.ServiceEndDate,
                            Deposit = (double)order.Deposit,
                            TotalPrice = (double)order.TotalPrice,
                            DiscountAmount = (double)order.DiscountAmount,
                            RemainAmount = (double)order.RemainAmount,
                            RewardPointGain = (int)order.RewardPointGain,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            Status = order.Status,
                            Reason = order.Description,
                            Service = serviceResModel
                        };
                        resList.Add(serviceOrderGetResModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblServiceOrders.PageSize)
                        .CurPage(listTblServiceOrders.CurrentPage)
                        .RecordCount(listTblServiceOrders.RecordCount)
                        .PageCount(listTblServiceOrders.PageCount);

                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    ServiceOrderListRes serviceOrderListRes = new()
                    {
                        Paging = paging,
                        ServiceOrderList = resList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderListRes;
                    result.Message = "Get service orders success.";
                    return result;

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get service orders failed.";
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

        public async Task<ResultModel> GetServiceOrderById(string token, Guid orderID)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                TblServiceOrder order = await _serviceOrderRepo.Get(orderID);
                if (order != null)
                {
                    TblService resService = await _serviceRepo.Get(order.ServiceId);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = resService.Id,
                        ServiceCode = resService.ServiceCode,
                        UserId = resService.UserId,
                        Rules = resService.Rules,
                        CreateDate = resService.CreateDate ?? DateTime.MinValue,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        Status = resService.Status,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };

                    TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };
                    ServiceOrderGetResModel serviceOrderGetResModel = new()
                    {
                        Id = order.Id,
                        OrderCode = order.OrderCode,
                        CreateDate = order.CreateDate,
                        ServiceStartDate = (DateTime)order.ServiceStartDate,
                        ServiceEndDate = (DateTime)order.ServiceEndDate,
                        Deposit = (double)order.Deposit,
                        TotalPrice = (double)order.TotalPrice,
                        DiscountAmount = (double)order.DiscountAmount,
                        RemainAmount = (double)order.RemainAmount,
                        RewardPointGain = (int)order.RewardPointGain,
                        RewardPointUsed = (int)order.RewardPointUsed,
                        Technician = technicianRes,
                        UserID = order.UserId,
                        TransportFee = (double)order.TransportFee,
                        Status = order.Status,
                        Reason = order.Description,
                        Service = serviceResModel
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderGetResModel;
                    result.Message = "Get service order success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Service order Id invalid.";
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

        public async Task<ResultModel> CancelServiceOrderById(string token, Guid orderID, string reason)
        {
            ResultModel result = new();
            try
            {
                TblServiceOrder? serviceOrder = await _serviceOrderRepo.Get(orderID);
                serviceOrder.Status = ServiceOrderStatus.CANCEL;
                serviceOrder.Description = reason;
                _ = await _serviceOrderRepo.UpdateServiceOrder(serviceOrder);

                _ = await _serviceRepo.ChangeServiceStatus(serviceOrder.ServiceId, ServiceStatus.REPROCESS);

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cancel service order success.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CancelSaleOrderById(string token, Guid saleOrderID, string reason)
        {
            ResultModel result = new();
            try
            {
                TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(saleOrderID);
                if (tblSaleOrder == null)
                {

                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Sale order Id invalid.";
                    return result;
                }
                ResultModel updateStatus = await _saleOrderRepo.UpdateSaleOrderStatus(saleOrderID, Status.CANCEL);
                tblSaleOrder.Description = reason;
                await _saleOrderRepo.UpdateSaleOrder(tblSaleOrder);
                if (updateStatus.IsSuccess == true)
                {
                    List<SaleOrderDetailResModel> tblSaleOrderDetails = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    foreach (SaleOrderDetailResModel i in tblSaleOrderDetails)
                    {
                        TblProductItemDetail? itemDetail = await _productItemDetailRepo.Get((Guid)i.ProductItemDetailID);
                        itemDetail.Quantity += i.Quantity;
                        _ = await _productItemDetailRepo.UpdateProductItemDetail(itemDetail);
                    }
                }
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cancel sale order success.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CancelRentOrderById(string token, Guid rentOrderID, string reason)
        {
            ResultModel result = new();
            try
            {
                TblRentOrder? rentOrder = await _rentOrderRepo.Get(rentOrderID);
                ResultModel chaneStatus = await _rentOrderRepo.UpdateRentOrderStatus(rentOrderID, Status.CANCEL);
                rentOrder.Description = reason;
                await _rentOrderRepo.UpdateRentOrder(rentOrder);
                if (chaneStatus.Code == 200)
                {
                    List<RentOrderDetailResModel> rentOrderDetail = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderID);
                    foreach (RentOrderDetailResModel i in rentOrderDetail)
                    {
                        TblProductItemDetail? itemDetail = await _productItemDetailRepo.Get(i.ProductItemDetail.Id);
                        itemDetail.Quantity += i.Quantity;
                        _ = await _productItemDetailRepo.UpdateProductItemDetail(itemDetail);
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    return result;
                }
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cancel sale order success.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CompleteServiceOrderStatus(string token, Guid serviceOrderID)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                bool update = await _serviceOrderRepo.CompleteServiceOrder(serviceOrderID);
                if (update == true)
                {
                    TblServiceOrder order = await _serviceOrderRepo.Get(serviceOrderID);
                    TblService resService = await _serviceRepo.Get(order.Id);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);

                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    TblTransaction tblTransaction = new()
                    {
                        Id = Guid.NewGuid(),
                        ServiceOrderId = order.Id,
                        Amount = order.TotalPrice,
                        DatetimePaid = currentTime,
                        Description = "Order completion refund.",
                        PaymentId = PaymentMethod.CASH,
                        Type = TransactionType.SERVICE_REFUND,
                        Status = TransactionStatus.REFUND

                    };

                    Guid insert = await _transactionRepo.Insert(tblTransaction);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = resService.Id,
                        ServiceCode = resService.ServiceCode,
                        UserId = resService.UserId,
                        Rules = resService.Rules,
                        CreateDate = resService.CreateDate ?? DateTime.MinValue,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        Status = resService.Status,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };

                    TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };
                    ServiceOrderGetResModel serviceOrderGetResModel = new()
                    {
                        Id = order.Id,
                        OrderCode = order.OrderCode,
                        CreateDate = order.CreateDate,
                        ServiceStartDate = (DateTime)order.ServiceStartDate,
                        ServiceEndDate = (DateTime)order.ServiceEndDate,
                        Deposit = (double)order.Deposit,
                        TotalPrice = (double)order.TotalPrice,
                        DiscountAmount = (double)order.DiscountAmount,
                        RemainAmount = (double)order.RemainAmount,
                        RewardPointGain = (int)order.RewardPointGain,
                        RewardPointUsed = (int)order.RewardPointUsed,
                        Technician = technicianRes,
                        UserID = order.UserId,
                        TransportFee = (double)order.TransportFee,
                        Status = order.Status,
                        Service = serviceResModel
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderGetResModel;
                    result.Message = "Complete service order success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Complete service order failed.";
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

        public async Task<ResultModel> SearchRentOrderDetail(string token, OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            ResultModel result = new();
            try
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


                Page<TblRentOrder> listTblRentOrder = await _rentOrderRepo.SearchRentOrder(model, pagingModel);
                var listRes = new List<RentOrderGroupModel>();
                if (listTblRentOrder == null)
                {
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                foreach (var tblRentOrder in listTblRentOrder.Results)
                {
                    if (tblRentOrder != null)
                    {
                        var rentOrderList = new List<RentOrderResModel>();
                        var tblRentGroup = await _rentOrderGroupRepo.Get((Guid)tblRentOrder.RentOrderGroupId);
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                        RentOrderResModel rentOrderResModel = new()
                        {
                            Id = tblRentOrder.Id,
                            UserId = tblRentOrder.UserId,
                            CreatedBy = tblRentOrder.CreatedBy,
                            IsTransport = tblRentOrder.IsTransport,
                            TransportFee = tblRentOrder.TransportFee,
                            StartRentDate = tblRentOrder.StartDateRent,
                            EndRentDate = tblRentOrder.EndDateRent,
                            Deposit = tblRentOrder.Deposit,
                            TotalPrice = tblRentOrder.TotalPrice,
                            Status = tblRentOrder.Status,
                            RemainMoney = tblRentOrder.RemainMoney,
                            RewardPointGain = tblRentOrder.RewardPointGain,
                            RewardPointUsed = tblRentOrder.RewardPointUsed,
                            RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                            DiscountAmount = tblRentOrder.DiscountAmount,
                            RecipientAddress = tblRentOrder.RecipientAddress,
                            RecipientDistrict = tblRentOrder.RecipientDistrict,
                            RecipientName = tblRentOrder.RecipientName,
                            RecipientPhone = tblRentOrder.RecipientPhone,
                            OrderCode = tblRentOrder.OrderCode,
                            CreateDate = tblRentOrder.CreateDate,
                            Reason = tblRentOrder.Description,
                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        rentOrderList.Add(rentOrderResModel);

                        RentOrderGroupModel rentOrderGroupModel = new()
                        {
                            ID = tblRentGroup.Id,
                            NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                            TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount,
                            RentOrderList = rentOrderList
                        };
                        listRes.Add(rentOrderGroupModel);
                    }
                }

                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listTblRentOrder.PageSize)
                    .CurPage(listTblRentOrder.CurrentPage)
                    .RecordCount(listTblRentOrder.RecordCount)
                    .PageCount(listTblRentOrder.PageCount);

                var newRes = new RentOrderByRangeDateResModel()
                {
                    Paging = paging,
                    RentOrderGroups = listRes
                };
                
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = newRes;
                result.Message = "Get rent order detail successful.";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetSaleOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            ResultModel result = new();
            try
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

                Page<TblSaleOrder> listTblSaleOrder = await _saleOrderRepo.GetSaleOrderByOrderCode(model, pagingModel);
                if (listTblSaleOrder == null)
                {
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                List<SaleOrderResModel> resList = new();
                if (listTblSaleOrder != null)
                {
                    foreach (TblSaleOrder order in listTblSaleOrder.Results)
                    {
                        List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(order.Id);
                        SaleOrderResModel saleOrderResModel = new()
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            IsTransport = order.IsTransport,
                            TransportFee = order.TransportFee,
                            CreateDate = (DateTime)order.CreateDate,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            RentOrderDetailList = saleOrderDetailResModels
                        };
                        resList.Add(saleOrderResModel);
                    }

                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblSaleOrder.PageSize)
                        .CurPage(listTblSaleOrder.CurrentPage)
                        .RecordCount(listTblSaleOrder.RecordCount)
                        .PageCount(listTblSaleOrder.PageCount);


                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    SaleOrderGetResModel saleOrderGetResModel = new()
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
            }
            return result;
        }

        public async Task<ResultModel> GetServiceOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER)
                    && !userRole.Equals(Commons.TECHNICIAN))
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
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.SearchServiceOrder(model, pagingModel);
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        ServiceResModel serviceResModel = new()
                        {
                            ID = resService.Id,
                            ServiceCode = resService.ServiceCode,
                            UserId = resService.UserId,
                            Rules = resService.Rules,
                            CreateDate = resService.CreateDate ?? DateTime.MinValue,
                            StartDate = resService.StartDate,
                            EndDate = resService.EndDate,
                            Name = resService.Name,
                            Phone = resService.Phone,
                            Email = resService.Email,
                            Address = resService.Address,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get(order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        ServiceOrderGetResModel serviceOrderGetResModel = new()
                        {
                            Id = order.Id,
                            OrderCode = order.OrderCode,
                            CreateDate = order.CreateDate,
                            ServiceStartDate = (DateTime)order.ServiceStartDate,
                            ServiceEndDate = (DateTime)order.ServiceEndDate,
                            Deposit = (double)order.Deposit,
                            TotalPrice = (double)order.TotalPrice,
                            DiscountAmount = (double)order.DiscountAmount,
                            RemainAmount = (double)order.RemainAmount,
                            RewardPointGain = (int)order.RewardPointGain,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            Status = order.Status,
                            Reason = order.Description,
                            Service = serviceResModel
                        };
                        resList.Add(serviceOrderGetResModel);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listTblServiceOrders.PageSize)
                        .CurPage(listTblServiceOrders.CurrentPage)
                        .RecordCount(listTblServiceOrders.RecordCount)
                        .PageCount(listTblServiceOrders.PageCount);

                    resList.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                    ServiceOrderListRes serviceOrderListRes = new()
                    {
                        Paging = paging,
                        ServiceOrderList = resList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceOrderListRes;
                    result.Message = "Get service orders success.";
                    return result;

                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = null;
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

        public async Task<ResultModel> GetRentOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel)
        {
            ResultModel result = new();
            try
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
                if (model.fromDate == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Error: fromDate null";
                    return result;
                }
                var fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                var toDate = new DateTime();
                if (model.toDate == null)
                {
                    toDate = fromDate.AddDays(1);
                }
                else
                {
                    toDate = ConvertUtil.convertStringToDateTime(model.toDate);
                }

                if (fromDate > toDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Error: fromDate > endDate";
                    return result;
                }


                Page<TblRentOrder> listTblRentOrder = await _rentOrderRepo.GetRentOrderByDate(fromDate, toDate, pagingModel);
                if (listTblRentOrder == null)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    return result;
                }
                var listRes = new List<RentOrderGroupModel>();
                foreach (var tblRentOrder in listTblRentOrder.Results)
                {
                    if (tblRentOrder != null)
                    {
                        var rentOrderList = new List<RentOrderResModel>();
                        var tblRentGroup = await _rentOrderGroupRepo.Get((Guid)tblRentOrder.RentOrderGroupId);
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                        RentOrderResModel rentOrderResModel = new()
                        {
                            Id = tblRentOrder.Id,
                            UserId = tblRentOrder.UserId,
                            CreatedBy = tblRentOrder.CreatedBy,
                            IsTransport = tblRentOrder.IsTransport,
                            TransportFee = tblRentOrder.TransportFee,
                            StartRentDate = tblRentOrder.StartDateRent,
                            EndRentDate = tblRentOrder.EndDateRent,
                            Deposit = tblRentOrder.Deposit,
                            TotalPrice = tblRentOrder.TotalPrice,
                            Status = tblRentOrder.Status,
                            RemainMoney = tblRentOrder.RemainMoney,
                            RewardPointGain = tblRentOrder.RewardPointGain,
                            RewardPointUsed = tblRentOrder.RewardPointUsed,
                            RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                            DiscountAmount = tblRentOrder.DiscountAmount,
                            RecipientAddress = tblRentOrder.RecipientAddress,
                            RecipientDistrict = tblRentOrder.RecipientDistrict,
                            RecipientName = tblRentOrder.RecipientName,
                            RecipientPhone = tblRentOrder.RecipientPhone,
                            OrderCode = tblRentOrder.OrderCode,
                            CreateDate = tblRentOrder.CreateDate,
                            Reason = tblRentOrder.Description,

                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        rentOrderList.Add(rentOrderResModel);

                        RentOrderGroupModel rentOrderGroupModel = new()
                        {
                            ID = tblRentGroup.Id,
                            NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                            TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount,
                            RentOrderList = rentOrderList
                        };
                        listRes.Add(rentOrderGroupModel);
                    }
                }

                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listTblRentOrder.PageSize)
                    .CurPage(listTblRentOrder.CurrentPage)
                    .RecordCount(listTblRentOrder.RecordCount)
                    .PageCount(listTblRentOrder.PageCount);

                var newRes = new RentOrderByRangeDateResModel()
                {
                    Paging = paging,
                    RentOrderGroups = listRes
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = newRes;
                result.Message = "Get rent order detail successful.";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        /*public async Task<ResultModel> GetRentOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
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
                if (model.fromDate == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Error: fromDate null";
                    return result;
                }
                var fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                var toDate = new DateTime();
                if (model.toDate == null)
                {
                    toDate = fromDate.AddDays(1);
                }
                else
                {
                    toDate = ConvertUtil.convertStringToDateTime(model.toDate);
                }

                if (fromDate > toDate)
                {
                    result.IsSuccess = false;
                    result.Message = "Error: fromDate > endDate";
                    return result;
                }

                Page<TblRentOrder> tblRentOrder = await _rentOrderRepo.GetRentOrderByDate(fromDate, toDate, pagingModel);
                List<RentOrderByDateResModel> resList = new();
                if (tblRentOrder.Results.Any())
                {
                    foreach (var order in tblRentOrder.Results)
                    {
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);

                        TblRentOrderGroup rentOrderGroup = await _rentOrderGroupRepo.Get((Guid)order.RentOrderGroupId);
                        List<TblRentOrder> listRentOrder = await _rentOrderRepo.GetRentOrdersByGroup(rentOrderGroup.Id);
                        RentOrderGroupModel rentOrderGroupModel = new();
                        if (listRentOrder.Any())
                        {
                            foreach (var o in listRentOrder)
                            {
                                List<RentOrderDetailResModel> rentOrderDetailModels = await _rentOrderDetailRepo.GetRentOrderDetails(o.Id);
                                rentOrderGroupModel.RentOrderList = new List<RentOrderResModel>();
                                RentOrderResModel rentOrderModel = new()
                                {
                                    Id = o.Id,
                                    UserId = o.UserId,
                                    CreatedBy = o.CreatedBy,
                                    CreateDate = o.CreateDate,
                                    IsTransport = o.IsTransport,
                                    TransportFee = o.TransportFee,
                                    StartRentDate = o.StartDateRent,
                                    EndRentDate = o.EndDateRent,
                                    Deposit = o.Deposit,
                                    TotalPrice = o.TotalPrice,
                                    Status = o.Status,
                                    RemainMoney = o.RemainMoney,
                                    RewardPointGain = o.RewardPointGain,
                                    RewardPointUsed = o.RewardPointUsed,
                                    RentOrderGroupID = o.RentOrderGroupId,
                                    DiscountAmount = o.DiscountAmount,
                                    RecipientAddress = o.RecipientAddress,
                                    RecipientDistrict = o.RecipientDistrict,
                                    RecipientName = o.RecipientName,
                                    RecipientPhone = o.RecipientPhone,
                                    OrderCode = o.OrderCode,
                                    RentOrderDetailList = rentOrderDetailModels
                                };
                                rentOrderGroupModel.RentOrderList.Add(rentOrderModel);
                            }
                        }

                        rentOrderGroupModel.ID = rentOrderGroup.Id;
                        rentOrderGroupModel.NumberOfOrder = (int)rentOrderGroup.NumberOfOrders;
                        rentOrderGroupModel.TotalGroupAmount = (double)rentOrderGroup.GroupTotalAmount;

                        RentOrderByDateResModel rentOrderbyDate = new()
                        {
                            Id = order.Id,
                            UserId = order.UserId,
                            CreatedBy = order.CreatedBy,
                            CreateDate = order.CreateDate,
                            IsTransport = order.IsTransport,
                            TransportFee = order.TransportFee,
                            StartRentDate = order.StartDateRent,
                            EndRentDate = order.EndDateRent,
                            Deposit = order.Deposit,
                            TotalPrice = order.TotalPrice,
                            Status = order.Status,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            RentOrderGroupID = order.RentOrderGroupId,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,

                            RentOrderDetailList = rentOrderDetailResModels,
                            RentOrderGroupModel = rentOrderGroupModel
                        };
                        resList.Add(rentOrderbyDate);                     
                    }
                }
                PaginationResponseModel paging = new PaginationResponseModel()
                            .PageSize(tblRentOrder.PageSize)
                            .CurPage(tblRentOrder.CurrentPage)
                            .RecordCount(tblRentOrder.RecordCount)
                            .PageCount(tblRentOrder.PageCount);

                resList.Sort((x, y) => y.EndRentDate.CompareTo(x.EndRentDate));

                RentOrderByRangeDateResModel rentOrderGroupResModel = new()
                {
                    Paging = paging,
                    RentOrderGroups = resList
                };


                result.IsSuccess = true;
                result.Code = 200;
                result.Data = rentOrderGroupResModel;
                result.Message = "Get rent orders successful.";
                return result;

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
