﻿using EntityFrameworkPaginateCore;
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
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
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
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using GreeenGarden.Data.Models.FileModel;
using TheArtOfDev.HtmlRenderer.Adapters;
using Microsoft.IdentityModel.Tokens;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using Microsoft.EntityFrameworkCore;
using static TheArtOfDev.HtmlRenderer.Adapters.RGraphicsPath;

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
        private readonly IServiceCalendarRepo _serCalendarRepo;
        private readonly IImageService _imageService;
        private readonly IEMailService _eMailService;
        private readonly IUserTreeRepo _userTreeRepo;
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
            IServiceCalendarRepo serCalendarRepo,
            IUserRepo userRepo,
            IImageService imageService,
            IEMailService eMailService,
            IUserTreeRepo userTreeRepo
            )
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
            _serCalendarRepo = serCalendarRepo;
            _imageService = imageService;
            _eMailService = eMailService;
            _userTreeRepo = userTreeRepo;
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
                string userName = _decodeToken.Decode(token, "username");
                ResultModel updateResult = await _rentOrderRepo.UpdateRentOrderStatus(rentOrderID, status, userName);
                if (status == Status.READY || status == Status.ACTIVE || status == Status.DISABLE
                    || status == Status.UNPAID || status == Status.PAID || status == Status.COMPLETED
                    || status == Status.CANCEL || status == Status.DELIVERY || status == Status.RENTING)
                {
                    if (updateResult.IsSuccess == true)
                    {
                        TblRentOrder rentOrder = await _rentOrderRepo.Get(rentOrderID);
                        var listRentOrderDetail = await _rentOrderDetailRepo.GetRentOrderDetailsByRentOrderID(rentOrderID);
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(rentOrderID);
                        if (status.Equals(Status.COMPLETED) || status.Equals(Status.CANCEL))
                        {
                            foreach (var i in listRentOrderDetail)
                            {
                                var productItemDetail = await _productItemDetailRepo.Get((Guid)i.ProductItemDetailId);
                                productItemDetail.Quantity += i.Quantity;
                                await _productItemDetailRepo.UpdateProductItemDetail(productItemDetail);
                            }
                        }
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)rentOrder.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
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
                            ContractURL = rentOrder.ContractUrl,
                            CareGuideURL = rentOrder.CareGuideUrl,
                            RecipientPhone = rentOrder.RecipientPhone,
                            RentOrderDetailList = rentOrderDetailResModels,
                            Reason = rentOrder.Description,
                            OrderCode = rentOrder.OrderCode,
                            CancelBy = rentOrder.CancelBy,
                            NameCancelBy = userCancelBy

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
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Status invalid";
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

                string userName = _decodeToken.Decode(token, "username");
                ResultModel updateResult = await _saleOrderRepo.UpdateSaleOrderStatus(saleOrderID, status, userName);
                if (updateResult.IsSuccess == true)
                {
                    TblSaleOrder saleOrder = await _saleOrderRepo.Get(saleOrderID);
                    if (status.Equals(Status.COMPLETED))
                    {
                        _ = await _rewardRepo.AddUserRewardPointByUserID((Guid)saleOrder.UserId, (int)saleOrder.RewardPointGain);
                    }
                    List<SaleOrderDetailResModel> saleOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(saleOrderID);
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)saleOrder.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        CareGuideURL = saleOrder.CareGuideUrl,
                        RecipientPhone = saleOrder.RecipientPhone,
                        RentOrderDetailList = saleOrderDetailResModels,
                        Reason = saleOrder.Description,
                        CancelBy = saleOrder.CancelBy,
                        IsTransport = saleOrder.IsTransport,
                        NameCancelBy = userCancelBy,
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
                            transportFee += (double)((itemDetail.TransportFee * item.Quantity));
                        }
                    }
                }
                if (rentOrderModel.IsTransport == true)
                {
                    TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(rentOrderModel.ShippingID);
                    transportFee += +tblShippingFee.FeeAmount;
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
                    OrderCode = await GenerateOrderCode(2)
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
                    FileData fileData = (FileData)GeneratePDF(tblRentOrder.Id).Result.Data;
                    ResultModel contractURLResult = await _imageService.UploadAPDF(fileData);
                    _ = await _rentOrderRepo.UpdateRentOrderContractUrl(tblRentOrder.Id, contractURLResult.Data.ToString());
                    TblUser tblUser = await _userRepo.Get(Guid.Parse(userID));
                    ResultModel resultGen = await GeneratePDF(tblRentOrder.Id);
                    FileData file = (FileData)resultGen.Data;
                    _ = await _eMailService.SendEmailRentOrderContract(tblUser.Mail, tblRentOrder.Id, file);

                    ResultModel resultCareGuideGen = await GenerateCareGuidePDF(tblRentOrder.Id, 1);
                    FileData fileCareGuide = (FileData)resultCareGuideGen.Data;
                    ResultModel careGuideURLResult = await _imageService.UploadAPDF(fileCareGuide);
                    _ = await _rentOrderRepo.UpdateRentOrderCareGuideUrl(tblRentOrder.Id, careGuideURLResult.Data.ToString());
                    _ = await _eMailService.SendEmailCareGuide(tblUser.Mail, tblRentOrder.Id, fileCareGuide, 1);

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create rent order failed.";
                    return result;
                }

                List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(tblRentOrder.Id);
                string userCancelBy = null;
                try
                {
                    userCancelBy = await _userRepo.GetFullNameByID((Guid)tblRentOrder.CancelBy);
                }
                catch (Exception)
                {
                    userCancelBy = null;
                }
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
                    CareGuideURL = tblRentOrder.CareGuideUrl,
                    ContractURL = tblRentOrder.ContractUrl,
                    RecipientPhone = tblRentOrder.RecipientPhone,
                    OrderCode = tblRentOrder.OrderCode,
                    CreateDate = tblRentOrder.CreateDate,
                    CancelBy = tblRentOrder.CancelBy,
                    NameCancelBy = userCancelBy,
                    Reason = tblRentOrder.Description,
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
                        totalAmountPerDay += (double)(item.Quantity * itemDetail.SalePrice);
                        totalQuantity += item.Quantity;

                        if (saleOrderModel.IsTransport == true)
                        {
                            transportFee += (double)((itemDetail.TransportFee * item.Quantity));
                        }
                    }
                }
                if (saleOrderModel.IsTransport == true)
                {
                    TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(saleOrderModel.ShippingID);
                    transportFee += tblShippingFee.FeeAmount;
                }

                discountAmount += (double)(saleOrderModel.RewardPointUsed * 1000);
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
                    OrderCode = await GenerateOrderCode(1)
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


                TblUser tblUser = await _userRepo.Get(Guid.Parse(userID));
                ResultModel resultCareGuideGen = await GenerateCareGuidePDF(tblSaleOrder.Id, 2);
                FileData fileCareGuide = (FileData)resultCareGuideGen.Data;
                _ = await _eMailService.SendEmailCareGuide(tblUser.Mail, tblSaleOrder.Id, fileCareGuide, 2);


                ResultModel careGuideURLResult = await _imageService.UploadAPDF(fileCareGuide);
                _ = await _saleOrderRepo.UpdateSaleOrderCareGuideURL(tblSaleOrder.Id, careGuideURLResult.Data.ToString());

                List<SaleOrderDetailResModel> rentOrderDetailResModels = await _saleOrderDetailRepo.GetSaleOrderDetails(tblSaleOrder.Id);
                string userCancelBy = null;
                try
                {
                    userCancelBy = await _userRepo.GetFullNameByID((Guid)tblSaleOrder.CancelBy);
                }
                catch (Exception)
                {
                    userCancelBy = null;
                }
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
                    CareGuideURL = tblSaleOrder.CareGuideUrl,
                    RecipientName = tblSaleOrder.RecipientName,
                    RecipientPhone = tblSaleOrder.RecipientPhone,
                    OrderCode = tblSaleOrder.OrderCode,
                    CancelBy = tblSaleOrder.CancelBy,
                    NameCancelBy = userCancelBy,
                    Reason = tblSaleOrder.Description,
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
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)tblRentOrder.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        CareGuideURL = tblRentOrder.CareGuideUrl,
                        RecipientName = tblRentOrder.RecipientName,
                        ContractURL = tblRentOrder.ContractUrl,
                        RecipientPhone = tblRentOrder.RecipientPhone,
                        OrderCode = tblRentOrder.OrderCode,
                        CreateDate = tblRentOrder.CreateDate,
                        Reason = tblRentOrder.Description,
                        CancelBy = tblRentOrder.CancelBy,
                        NameCancelBy = userCancelBy,
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
                                string userCancelBy = null;
                                try
                                {
                                    userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                                }
                                catch (Exception)
                                {
                                    userCancelBy = null;
                                }
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
                                    CareGuideURL = order.CareGuideUrl,
                                    RecipientDistrict = order.RecipientDistrict,
                                    RecipientName = order.RecipientName,
                                    ContractURL = order.ContractUrl,
                                    RecipientPhone = order.RecipientPhone,
                                    OrderCode = order.OrderCode,
                                    CreateDate = order.CreateDate,
                                    Reason = order.Description,
                                    CancelBy = order.CancelBy,
                                    NameCancelBy = userCancelBy,
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
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)tblSaleOrder.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        CareGuideURL = tblSaleOrder.CareGuideUrl,
                        RecipientDistrict = tblSaleOrder.RecipientDistrict,
                        RecipientName = tblSaleOrder.RecipientName,
                        RecipientPhone = tblSaleOrder.RecipientPhone,
                        OrderCode = tblSaleOrder.OrderCode,
                        Reason = tblSaleOrder.Description,
                        NameCancelBy = userCancelBy,
                        CancelBy = tblSaleOrder.CancelBy,

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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = order.CareGuideUrl,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            NameCancelBy = userCancelBy,
                            CancelBy = order.CancelBy,
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
                                string userCancelBy = null;
                                try
                                {
                                    userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                                }
                                catch (Exception)
                                {
                                    userCancelBy = null;
                                }
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
                                    CareGuideURL = order.CareGuideUrl,
                                    RecipientAddress = order.RecipientAddress,
                                    RecipientDistrict = order.RecipientDistrict,
                                    RecipientName = order.RecipientName,
                                    ContractURL = order.ContractUrl,
                                    RecipientPhone = order.RecipientPhone,
                                    CancelBy = order.CancelBy,
                                    NameCancelBy = userCancelBy,
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
                            CreateDate = (DateTime)tblRentGroup.CreateDate,
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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = order.CareGuideUrl,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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

        private async Task<string> GenerateOrderCode(int flag)
        {
            try
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                string frontCode = null;
                if (flag == 1) frontCode = "SALE_";
                if (flag == 2) frontCode = "RENT_";
                if (flag == 3) frontCode = "CARE_";
                string orderCode = frontCode + currentTime.ToString("ddMMyyyy") + "_";
                bool dup = true;
                while (dup == true)
                {
                    Random random = new();
                    orderCode += new string(Enumerable.Repeat("0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray());
                    bool checkCodeDup = await _rentOrderRepo.checkCodeDup(orderCode, flag);
                    dup = checkCodeDup != false;
                }
                return orderCode;
            }
            catch (Exception e)
            {
                throw e;
            }
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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = order.CareGuideUrl,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            ContractURL = order.ContractUrl,
                            RecipientPhone = order.RecipientPhone,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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
                            transportFee += (double)((itemDetail.TransportFee * item.Quantity));
                        }
                    }
                }
                if (rentOrderModel.IsTransport == true)
                {
                    TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(rentOrderModel.ShippingID);
                    transportFee += tblShippingFee.FeeAmount;
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
                        totalAmountPerDay += (double)(item.Quantity * itemDetail.SalePrice);
                        totalQuantity += item.Quantity;
                        if (saleOrderModel.IsTransport == true)
                        {
                            transportFee += (double)((itemDetail.TransportFee * item.Quantity));
                        }
                    }
                }
                if (saleOrderModel.IsTransport == true)
                {
                    TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(saleOrderModel.ShippingID);
                    transportFee += tblShippingFee.FeeAmount;
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
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)tblRentOrder.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        CareGuideURL = tblRentOrder.CareGuideUrl,
                        RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                        DiscountAmount = tblRentOrder.DiscountAmount,
                        RecipientAddress = tblRentOrder.RecipientAddress,
                        RecipientDistrict = tblRentOrder.RecipientDistrict,
                        RecipientName = tblRentOrder.RecipientName,
                        ContractURL = tblRentOrder.ContractUrl,
                        CancelBy = tblRentOrder.CancelBy,
                        NameCancelBy = userCancelBy,
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
                        var tblProductItem = await _productItemRepo.Get(detail.ProductItemId);
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
                            CareGuide = tblProductItem.CareGuide,
                            TransportFee = detail.TransportFee,
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
                    OrderCode = await GenerateOrderCode(3),
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
                Guid insert = await _serviceOrderRepo.Insert(tblServiceOrder);
                _ = await _serviceRepo.ChangeServiceStatus(tblServiceOrder.ServiceId, ServiceStatus.CONFIRMED);
                if (insert != Guid.Empty)
                {
                    //
                    TblUser tblUser = await _userRepo.Get(tblService.UserId);
                    ResultModel resultCareGuideGen = await GenerateServiceCareGuidePDF(tblServiceOrder.Id);
                    FileData fileCareGuide = (FileData)resultCareGuideGen.Data;
                    _ = await _eMailService.SendEmailServiceCareGuide(tblUser.Mail, tblServiceOrder.Id, fileCareGuide);


                    ResultModel careGuideURLResult = await _imageService.UploadAPDF(fileCareGuide);
                    _ = await _serviceOrderRepo.UpdateServiceOrderCareGuide(tblServiceOrder.Id, careGuideURLResult.Data.ToString());

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
                        TechnicianID = (Guid)tblServiceOrderGet.TechnicianId,
                        ServiceID = tblServiceOrderGet.ServiceId,
                        UserID = tblServiceOrderGet.UserId,
                        TransportFee = (double)tblServiceOrderGet.TransportFee,
                        CareGuideURL = tblServiceOrderGet.CareGuideUrl,
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
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
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
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };
                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuide = order.CareGuideUrl,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
                        int userCurrentPoint = await _rewardRepo.GetUserRewardPoint(resService.UserId);
                        ServiceResModel serviceResModel = new()
                        {
                            ID = resService.Id,
                            ServiceCode = resService.ServiceCode,
                            UserId = resService.UserId,
                            DistrictID = resService.DistrictId,
                            IsTransport = resService.IsTransport,
                            RewardPointUsed = resService.RewardPointUsed,
                            TransportFee = resService.TransportFee,
                            ServiceOrderID = order.Id,
                            Rules = resService.Rules,
                            CreateDate = resService.CreateDate ?? DateTime.MinValue,
                            StartDate = resService.StartDate,
                            EndDate = resService.EndDate,
                            UserCurrentPoint = userCurrentPoint,
                            Name = resService.Name,
                            Phone = resService.Phone,
                            Email = resService.Email,
                            Address = resService.Address,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
                            CareGuide = order.CareGuideUrl,
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
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
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
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
                            CareGuide = order.CareGuideUrl,
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

                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
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
                        Reason = resService.Reason,
                        CancelBy = resService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Address = resService.Address,
                        Status = resService.Status,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };

                    TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        CancelBy = order.CancelBy,
                        NameCancelBy = userCancelBy,
                        CareGuide = order.CareGuideUrl,
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
                string userName = _decodeToken.Decode(token, "username");
                var user = await _userRepo.GetCurrentUser(userName);
                TblServiceOrder? serviceOrder = await _serviceOrderRepo.Get(orderID);
                serviceOrder.Status = ServiceOrderStatus.CANCEL;
                serviceOrder.CancelBy = user.Id;
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

                string userName = _decodeToken.Decode(token, "username");
                ResultModel updateStatus = await _saleOrderRepo.UpdateSaleOrderStatus(saleOrderID, Status.CANCEL, userName);
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
                    await _rewardRepo.AddUserRewardPointByUserID((Guid)tblSaleOrder.UserId, (int)tblSaleOrder.RewardPointUsed);
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

                string userName = _decodeToken.Decode(token, "username");
                ResultModel chaneStatus = await _rentOrderRepo.UpdateRentOrderStatus(rentOrderID, Status.CANCEL, userName);
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
                    await _rewardRepo.AddUserRewardPointByUserID((Guid)rentOrder.UserId, (int)rentOrder.RewardPointUsed);
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
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
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
                        Reason = resService.Reason,
                        CancelBy = resService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };

                    TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };
                    string userCancelBy = null;
                    try
                    {
                        userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                    }
                    catch (Exception)
                    {
                        userCancelBy = null;
                    }
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
                        NameCancelBy = userCancelBy,
                        Technician = technicianRes,
                        CareGuide = order.CareGuideUrl,
                        UserID = order.UserId,
                        TransportFee = (double)order.TransportFee,
                        Status = order.Status,
                        Reason = order.Description,
                        CancelBy = order.CancelBy,
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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)tblRentOrder.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = tblRentOrder.CareGuideUrl,
                            NameCancelBy = userCancelBy,
                            RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                            DiscountAmount = tblRentOrder.DiscountAmount,
                            RecipientAddress = tblRentOrder.RecipientAddress,
                            RecipientDistrict = tblRentOrder.RecipientDistrict,
                            RecipientName = tblRentOrder.RecipientName,
                            ContractURL = tblRentOrder.ContractUrl,
                            RecipientPhone = tblRentOrder.RecipientPhone,
                            OrderCode = tblRentOrder.OrderCode,
                            CreateDate = tblRentOrder.CreateDate,
                            Reason = tblRentOrder.Description,
                            CancelBy = tblRentOrder.CancelBy,
                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        rentOrderList.Add(rentOrderResModel);

                        RentOrderGroupModel rentOrderGroupModel = new();
                        rentOrderGroupModel.ID = tblRentGroup.Id;
                        rentOrderGroupModel.NumberOfOrder = (int)tblRentGroup.NumberOfOrders;
                        rentOrderGroupModel.TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount;
                        if (tblRentGroup.CreateDate != null) rentOrderGroupModel.CreateDate = (DateTime)tblRentGroup.CreateDate;
                        if (tblRentGroup.CreateDate == null) rentOrderGroupModel.CreateDate = new DateTime();
                        rentOrderGroupModel.RentOrderList = rentOrderList;
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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = order.CareGuideUrl,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            RecipientPhone = order.RecipientPhone,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
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
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            Status = resService.Status,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuide = order.CareGuideUrl,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            Status = order.Status,
                            Reason = order.Description,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)tblRentOrder.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
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
                            NameCancelBy = userCancelBy,
                            RemainMoney = tblRentOrder.RemainMoney,
                            CareGuideURL = tblRentOrder.CareGuideUrl,
                            RewardPointGain = tblRentOrder.RewardPointGain,
                            RewardPointUsed = tblRentOrder.RewardPointUsed,
                            RentOrderGroupID = tblRentOrder.RentOrderGroupId,
                            DiscountAmount = tblRentOrder.DiscountAmount,
                            RecipientAddress = tblRentOrder.RecipientAddress,
                            RecipientDistrict = tblRentOrder.RecipientDistrict,
                            RecipientName = tblRentOrder.RecipientName,
                            ContractURL = tblRentOrder.ContractUrl,
                            RecipientPhone = tblRentOrder.RecipientPhone,
                            OrderCode = tblRentOrder.OrderCode,
                            CreateDate = tblRentOrder.CreateDate,
                            CancelBy = tblRentOrder.CancelBy,
                            Reason = tblRentOrder.Description,

                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        rentOrderList.Add(rentOrderResModel);

                        RentOrderGroupModel rentOrderGroupModel = new()
                        {
                            ID = tblRentGroup.Id,
                            NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                            CreateDate = (DateTime)tblRentGroup.CreateDate,
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

        public async Task<ResultModel> UpdateDateTakecare(string token, UpdateDateTakecareModel model)
        {
            var result = new ResultModel();
            try
            {
                DateTime serviceEndDate = ConvertUtil.convertStringToDateTime(model.endDate);
                var order = await _serviceOrderRepo.Get(model.orderID);
                if (order == null)
                {
                    result.IsSuccess = true;
                    result.Message = "Không tìm thấy đơn hàng";
                    return result;
                }
                order.ServiceEndDate = serviceEndDate;

                await _serviceOrderRepo.UpdateServiceOrder(order);
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cập nhật thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateServiceOrderStatus(string token, UpdateServiceOrderStatusModel model)
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
            var result = new ResultModel();
            try
            {
                var order = await _serviceOrderRepo.Get(model.orderID);

                if (order == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy đơn hàng";
                    return result;
                }
                if (!model.status.Equals(ServiceOrderStatus.UNPAID)
                    && !model.status.Equals(ServiceOrderStatus.READY)
                    && !model.status.Equals(ServiceOrderStatus.PAID)
                    && !model.status.Equals(ServiceOrderStatus.COMPLETED)
                    && !model.status.Equals(ServiceOrderStatus.CANCEL))
                {
                    result.IsSuccess = false;
                    result.Message = "Status wrong!";
                    return result;
                }

                order.Status = model.status;
                await _serviceOrderRepo.UpdateServiceOrder(order);


                if (model.status == ServiceOrderStatus.COMPLETED)
                {
                    var user = await _userRepo.Get(order.UserId);
                    var service = await _serviceRepo.Get(order.ServiceId);
                    var serviceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);

                    await _serviceRepo.ChangeServiceStatus(service.Id, Status.COMPLETED);

                    ResultModel resultCareGuideGen = await GenerateCareGuidePDFForService(serviceDetail);
                    FileData fileCareGuide = (FileData)resultCareGuideGen.Data;
                    _ = await _eMailService.SendEmailCareGuideForService(user.Mail, serviceDetail, fileCareGuide);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cập nhật thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GeneratePDF(Guid orderCode)
        {
            var result = new ResultModel();
            try
            {
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(orderCode);
                if (tblRentOrder == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "OrderID invalid.";
                    return result;
                }
                TblUser tblUser = await _userRepo.Get((Guid)tblRentOrder.UserId);
                List<TblRentOrderDetail> tblRentOrderDetails = await _rentOrderDetailRepo.GetRentOrderDetailsByRentOrderID(tblRentOrder.Id);
                var document = new PdfDocument();
                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";
                htmlContent += "<div style='width:100%; font: bold'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</h2>";
                htmlContent += "<h3 style='width:100%; text-align:center'>Độc lập – Tự do – Hạnh phúc</h3>";
                htmlContent += "<p style='width:100%; text-align:right'>Hồ Chí Minh, " + tblRentOrder.CreateDate.Value.ToString("dd/MM/yyyy") + "</p>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HỢP ĐỒNG THUÊ CÂY </h2>";

                htmlContent += "<p style='width:100%;'>Các bên tham gia hợp đồng gồm:</p>";

                htmlContent += "<table style='width:100%'>";
                htmlContent += "<tr>";

                htmlContent += "<td style='vertical-align: top;'>";
                htmlContent += "<h3 style='width:100%;'>BÊN CHO THUÊ (Bên A)</h3>";
                htmlContent += "<p>Tên: Green Garden</p>";
                htmlContent += "<p>Địa chỉ: Đại Học FPT</p>";
                htmlContent += "<p>Điện thoại: 0909000999</p>";
                htmlContent += "</td>";

                htmlContent += "<td style=' vertical-align: top;'>";
                htmlContent += "<h3 style='width:100%;'>BÊN THUÊ (Bên B) </h3>";
                htmlContent += "<p>Tên khách hàng: " + tblUser.FullName + " </p>";
                htmlContent += "<p>Địa chỉ: " + tblUser.Address + " </p>";
                htmlContent += "<p>Điện thoại:" + tblUser.Phone + " <br></p>";
                htmlContent += "</td>";

                htmlContent += "</tr>";
                htmlContent += "</table>";

                htmlContent += "<p>Hai bên thống nhất thỏa thuận nội dung hợp đồng như sau:</p>";
                htmlContent += "<h3>Điều 1: Điều khoản chung</h3>";
                htmlContent += "<p>- Nếu trong quá trình thuê cây có vấn đề như hư, héo, chết, .... thì bên B sẽ chịu trách nhiệm hoàn toàn tuỳ thuộc vào tình trạng của cây.<br>" +
                    "- Bên B phải kiểm tra kĩ cây trước khi nhận. Nếu có vấn đề thì phải báo cho bên A, cây sẽ được đổi cây mới không phụ thu bất kì chi phí nào.<br>" +
                    "- Nếu bên B không kiểm tra kĩ cây trước khi nhận thì khi cây có vấn đề thì bên B phải chịu trách nhiệm.<br>" +
                    "- Nếu cây không được trả đúng hạn thì sẽ phụ thu thêm tiền cho các ngày tiếp theo đến khi nào cây được trả.<br>" +
                    "- Bên B có thể tự mình gia hạn thêm thời gian thuê trên hệ thống.<br>" +
                    "- Khi gia hạn thuê thì chỉ được chọn những cây đang thuê, không được thêm bất cứ cây nào khác nếu thêm thì sẽ tạo đơn hàng mới.<br>" +
                    "- Khi thuê cây bên B phải cọc 20% giá trị đơn hàng. Khi trả cây thì bên A sẽ trả lại cọc cho bên B.<br>" +
                    "- Bên B đặt đơn xong vui lòng thanh toán cọc. Đơn hàng chỉ được giao khi bên B đã thanh toán cọc.<br>" +
                    "- Nếu trong quá trình thuê cây có vấn đề thì phải báo gấp cho bên A biết để kịp thời cứu chữa.<br></p>";
                int count = 2;
                foreach (TblRentOrderDetail tblRentOrderDetail in tblRentOrderDetails)
                {
                    TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get((Guid)tblRentOrderDetail.ProductItemDetailId);
                    TblProductItem tblProductItem = await _productItemRepo.Get(tblProductItemDetail.ProductItemId);
                    if (!String.IsNullOrEmpty(tblProductItem.Rule))
                    {
                        htmlContent += "<h3>Điều " + count + ": Đối với cây " + tblProductItem.Name + "</h3>";


                        string a = tblProductItem.Rule;
                        List<string> splitted = a.Split('.').ToList();

                        foreach (string b in splitted)
                        {
                            if (!b.Equals(splitted.Last()))
                            {
                                htmlContent += "<p>-" + b + ".</p>";
                            }

                        }
                        count++;
                    }
                }

                htmlContent += "<h3>Những điều khoản trên được áp dụng với những sản phẩm:</h3>";


                htmlContent += "<table style ='width:100%; border: 1px solid #000; border-collapse: collapse'>";
                htmlContent += "<thead style='font-weight:bold; border-collapse: collapse'>";
                htmlContent += "<tr>";
                htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'> Tên sản phẩm </td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'> Kích thước </td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Giá thuê 1 ngày</td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Số lượng</td >";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Tổng tiền 1 ngày</td>";
                htmlContent += "</tr>";
                htmlContent += "</thead >";
                htmlContent += "<tbody>";

                foreach (TblRentOrderDetail tblRentOrderDetail in tblRentOrderDetails)
                {
                    TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get((Guid)tblRentOrderDetail.ProductItemDetailId);
                    TblSize tblSize = await _sizeRepo.Get(tblProductItemDetail.SizeId);
                    TblProductItem tblProductItem = await _productItemRepo.Get(tblProductItemDetail.ProductItemId);
                    htmlContent += "<tr>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblProductItem.Name + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblSize.Name + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.RentPricePerUnit + "</td >";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.Quantity + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.TotalPrice + "</td >";
                    htmlContent += "</tr>";
                }
                htmlContent += "</tbody>";
                htmlContent += "</table>";

                htmlContent += "<p style='text-align:right;'>Thuê từ " + tblRentOrder.StartDateRent.ToString("dd/MM/yyyy") + " đến: " + tblRentOrder.EndDateRent.ToString("dd/MM/yyyy") + "</p>";
                htmlContent += "<p style='text-align:right;'>Số tiền được giảm: " + tblRentOrder.DiscountAmount + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Phí vận chuyển: " + tblRentOrder.TransportFee + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Tổng cộng: " + tblRentOrder.TotalPrice + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Tiền cọc: " + tblRentOrder.Deposit + "đ</p>";


                htmlContent += "<table style='width:100%'>";
                htmlContent += "<tr>";

                htmlContent += "<td style='text-align:center;'>";
                htmlContent += "<h3 style='width:100%;'>ĐẠI DIỆN BÊN A</h3>";
                htmlContent += "<p> Green Garden  </p>";
                htmlContent += "</td>";

                htmlContent += "<td style=' text-align:center;'>";
                htmlContent += "<h3 style='width:100%;'> ĐẠI DIỆN BÊN B </h3>";
                htmlContent += "<p>" + tblUser.FullName + "</p>";
                htmlContent += "</td>";

                htmlContent += "</tr>";
                htmlContent += "</table>";



                htmlContent += "</div>";
                htmlContent += "</body>";
                htmlContent += "</html>";
                PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
                byte[]? response = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = new FileData(response, "application/pdf", Guid.NewGuid().ToString() + ".pdf");
                result.Message = "Generate PDF successful.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRentOrderGroupByOrderID(string token, Guid orderID)
        {
            var result = new ResultModel();
            try
            {
                var tblRentOrder = await _rentOrderRepo.Get(orderID);


                List<RentOrderGroupModel> listGroup = new();
                var tblRentGroup = await _rentOrderGroupRepo.Get((Guid)tblRentOrder.RentOrderGroupId);
                List<TblRentOrder> listRentOrders = await _rentOrderRepo.GetRentOrdersByGroup((Guid)tblRentOrder.RentOrderGroupId);
                List<RentOrderResModel> resList = new();
                if (listRentOrders.Any())
                {
                    foreach (TblRentOrder order in listRentOrders)
                    {
                        List<RentOrderDetailResModel> rentOrderDetailResModels = await _rentOrderDetailRepo.GetRentOrderDetails(order.Id);
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuideURL = order.CareGuideUrl,
                            RemainMoney = order.RemainMoney,
                            RewardPointGain = order.RewardPointGain,
                            RewardPointUsed = order.RewardPointUsed,
                            RentOrderGroupID = order.RentOrderGroupId,
                            DiscountAmount = order.DiscountAmount,
                            RecipientAddress = order.RecipientAddress,
                            RecipientDistrict = order.RecipientDistrict,
                            RecipientName = order.RecipientName,
                            ContractURL = order.ContractUrl,
                            RecipientPhone = order.RecipientPhone,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
                            OrderCode = order.OrderCode,
                            Reason = order.Description,

                            RentOrderDetailList = rentOrderDetailResModels
                        };
                        resList.Add(rentOrderResModel);
                    }
                }

                var resListSortDate = resList.OrderBy(x => x.CreateDate).ToList();

                RentOrderGroupModel rentOrderGroupModel = new()
                {
                    ID = tblRentGroup.Id,
                    NumberOfOrder = (int)tblRentGroup.NumberOfOrders,
                    CreateDate = (DateTime)tblRentGroup.CreateDate,
                    TotalGroupAmount = (double)tblRentGroup.GroupTotalAmount,
                    RentOrderList = resListSortDate
                };



                result.Code = 200;
                result.IsSuccess = true;
                result.Data = rentOrderGroupModel;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetServiceOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel)
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

            try
            {
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.GetAllServiceOrderByRangDate(pagingModel, fromDate, toDate);
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
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
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuide = order.CareGuideUrl,
                            UserID = order.UserId,
                            TransportFee = (double)order.TransportFee,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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

                    resList.Sort((x, y) => y.ServiceEndDate.CompareTo(x.ServiceEndDate));

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

        public async Task<ResultModel> GetServiceOrderByTechnicianToday(string token, PaginationRequestModel pagingModel, Guid technicianID, string status, bool nextDay)
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
                Page<TblServiceOrder> listTblServiceOrders = await _serviceOrderRepo.GetServiceOrderByTechnicianToday(pagingModel, technicianID, status, nextDay);
                if (listTblServiceOrders == null)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Empty calendar in today.";
                    result.Data = new ServiceOrderListRes();
                    return result;

                }
                List<ServiceOrderGetResModel> resList = new();
                if (listTblServiceOrders != null)
                {
                    foreach (TblServiceOrder order in listTblServiceOrders.Results)
                    {
                        TblService resService = await _serviceRepo.Get(order.ServiceId);
                        List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)resService.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
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
                            Reason = resService.Reason,
                            CancelBy = resService.CancelBy,
                            NameCancelBy = nameCancelBy,
                            TechnicianID = resService.TechnicianId,
                            TechnicianName = resService.TechnicianName,
                            ServiceDetailList = resServiceDetail
                        };

                        TblUser technicianGet = await _userRepo.Get((Guid)order.TechnicianId);
                        ServiceOrderTechnician technicianRes = new()
                        {
                            TechnicianID = technicianGet.Id,
                            TechnicianUserName = technicianGet.UserName,
                            TechnicianFullName = technicianGet.FullName,
                            TechnicianAddress = technicianGet.Address,
                            TechnicianMail = technicianGet.Mail,
                            TechnicianPhone = technicianGet.Phone
                        };
                        string userCancelBy = null;
                        try
                        {
                            userCancelBy = await _userRepo.GetFullNameByID((Guid)order.CancelBy);
                        }
                        catch (Exception)
                        {
                            userCancelBy = null;
                        }
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
                            CareGuide = order.CareGuideUrl,
                            RewardPointUsed = (int)order.RewardPointUsed,
                            Technician = technicianRes,
                            UserID = order.UserId,
                            CancelBy = order.CancelBy,
                            NameCancelBy = userCancelBy,
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

        public async Task<ResultModel> GenerateCareGuidePDF(Guid orderCode, int flag)
        {
            var result = new ResultModel();
            try
            {
                var itemDetails = new List<TblProductItemDetail>();
                var productItems = new List<TblProductItem>();
                if (flag == 1)
                {
                    TblRentOrder tblRentOrder = await _rentOrderRepo.Get(orderCode);
                    if (tblRentOrder == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "OrderID invalid.";
                        return result;
                    }
                    itemDetails = await _productItemDetailRepo.GetItemDetailsByRentOrderID(orderCode);
                    productItems = await _productItemRepo.GetItemsByItemDetail(itemDetails);
                }
                if (flag == 2)
                {
                    var tblSaleOrder = await _saleOrderRepo.Get(orderCode);
                    if (tblSaleOrder == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "OrderID invalid.";
                        return result;
                    }
                    itemDetails = await _productItemDetailRepo.GetItemDetailsBySaleOrderID(orderCode);
                    productItems = await _productItemRepo.GetItemsByItemDetail(itemDetails);
                }

                var document = new PdfDocument();
                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";
                htmlContent += "<div style='width:100%; font: bold'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HƯỚNG DẪN CHĂM SÓC CÂY </h2>";

                int count = 1;
                foreach (var productItem in productItems)
                {
                    if (!String.IsNullOrEmpty(productItem.CareGuide))
                    {
                        htmlContent += count + "<h3> Hướng dẫn chăm sóc với " + productItem.Name + "</h3>";


                        string a = productItem.CareGuide;
                        List<string> splitted = a.Split('.').ToList();

                        foreach (string b in splitted)
                        {
                            if (!b.Equals(splitted.Last()))
                            {
                                htmlContent += "<p>-" + b + ".</p>";
                            }

                        }
                    }
                    count++;
                }
                htmlContent += "<h4 style='width:100%;text-align:center'>Quý khách vui lòng làm theo hướng dẫn. Nếu có gì thắc mắc xin liên hệ 0833 449 449 </h2>";

                PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
                byte[]? response = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = new FileData(response, "application/pdf", Guid.NewGuid().ToString() + ".pdf");
                result.Message = "Generate PDF successful.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GenerateCareGuidePDFForService(List<ServiceDetailResModel> listItem)
        {
            var result = new ResultModel();
            try
            {
                var document = new PdfDocument();
                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";
                htmlContent += "<div style='width:100%; font: bold'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HƯỚNG DẪN CHĂM SÓC CÂY </h2>";

                int count = 1;
                foreach (var item in listItem)
                {
                    var service = await _serviceRepo.Get(item.ServiceID);
                    if (!String.IsNullOrEmpty(item.CareGuide))
                    {
                        htmlContent += count + "<h3> Hướng dẫn chăm sóc với " + item.TreeName + "</h3>";


                        string a = item.CareGuide;
                        List<string> splitted = a.Split('.').ToList();

                        foreach (string b in splitted)
                        {
                            if (!b.Equals(splitted.Last()))
                            {
                                htmlContent += "<p>-" + b + ".</p>";
                            }

                        }
                    }
                    count++;
                }
                htmlContent += "<h4 style='width:100%;text-align:center'>Quý khách vui lòng làm theo hướng dẫn. Nếu có gì thắc mắc xin liên hệ 0833 449 449 </h2>";

                PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
                byte[]? response = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = new FileData(response, "application/pdf", Guid.NewGuid().ToString() + ".pdf");
                result.Message = "Generate PDF successful.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateCareGuideByTechnician(string token, UpdateCareGuideByTechnModel model)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
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
            var result = new ResultModel();
            try
            {
                string userID = _decodeToken.Decode(token, "userid");

                var serviceOrder = await _serviceOrderRepo.Get(model.OrderID);
                if (!serviceOrder.TechnicianId.Equals(Guid.Parse(userID)))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
                var check = await _serviceDetailRepo.UpdateCareGuideByUserTree(model);
                if (check)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = "Update successful";
                }
                else
                {

                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Data = "Update fail";
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

        public async Task<ResultModel> GenerateServiceCareGuidePDF(Guid orderCode)
        {
            var result = new ResultModel();
            try
            {
                var tblServiceOrder = await _serviceOrderRepo.Get(orderCode);
                if (tblServiceOrder == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "OrderID invalid.";
                    return result;
                }
                var service = await _serviceRepo.GetServiceByServiceOrderID(orderCode);
                var serviceDetails = await _serviceDetailRepo.GetServiceDetailsByServiceID(service.Id);


                var document = new PdfDocument();
                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";
                htmlContent += "<div style='width:100%; font: bold'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HƯỚNG DẪN CHĂM SÓC CÂY </h2>";

                int count = 1;
                foreach (var serviceDetail in serviceDetails)
                {
                    if (!String.IsNullOrEmpty(serviceDetail.CareGuide))
                    {
                        var userTree = await _userTreeRepo.Get((Guid)serviceDetail.UserTreeId);

                        htmlContent += count + "<h3> Hướng dẫn chăm sóc với " + userTree.TreeName + "</h3>";


                        string a = serviceDetail.CareGuide;
                        List<string> splitted = a.Split('.').ToList();

                        foreach (string b in splitted)
                        {
                            if (!b.Equals(splitted.Last()))
                            {
                                htmlContent += "<p>-" + b + ".</p>";
                            }

                        }
                    }
                    count++;
                }
                htmlContent += "<h4 style='width:100%;text-align:center'>Quý khách vui lòng làm theo hướng dẫn. Nếu có gì thắc mắc xin liên hệ 0833 449 449 </h2>";

                PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
                byte[]? response = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = new FileData(response, "application/pdf", Guid.NewGuid().ToString() + ".pdf");
                result.Message = "Generate PDF successful.";
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
