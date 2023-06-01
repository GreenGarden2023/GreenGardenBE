using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboOrder;
using GreeenGarden.Data.Repositories.TakecareComboOrderRepo;
using GreeenGarden.Data.Repositories.TakecareComboServiceRepo;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.TakecareComboServiceDetailRepo;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Models.PaginationModel;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Models.FileModel;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Business.Service.ImageService;

namespace GreeenGarden.Business.Service.TakecareComboOrderService
{
    public class TakecareComboOrderService : ITakecareComboOrderService
    {
        private readonly ITakecareComboOrderRepo _takecareComboOrderRepo;
        private readonly ITakecareComboServiceRepo _takecareComboServiceRepo;
        private readonly ITakecareComboServiceDetailRepo _takecareComboServiceDetailRepo;
        private readonly IUserRepo _userRepo;
        private readonly IEMailService _emailService;
        private readonly IImageService _imageService;
        private readonly DecodeToken _decodeToken;
        public TakecareComboOrderService(ITakecareComboOrderRepo takecareComboOrderRepo, ITakecareComboServiceRepo takecareComboServiceRepo,
            ITakecareComboServiceDetailRepo takecareComboServiceDetailRepo, IUserRepo userRepo, IEMailService emailService,
            IImageService imageService)
        {
            _takecareComboOrderRepo = takecareComboOrderRepo;
            _takecareComboServiceRepo = takecareComboServiceRepo;
            _decodeToken = new DecodeToken();
            _takecareComboServiceDetailRepo = takecareComboServiceDetailRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _imageService = imageService;
        }

        public async Task<ResultModel> CancelTakecareComboOrder(Guid id, string cancelReason, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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
                bool update = await _takecareComboOrderRepo.CancelOrder(id, cancelReason, userID);
                if (update == true)
                {
                    TakecareComboOrderModel takecareComboOrderModel = await GetTakecareComboOrder(id);
                    var updateTakecareComboServiceStatus = await _takecareComboServiceRepo.ChangeTakecareComboServiceStatus(takecareComboOrderModel.TakecareComboService.Id, TakecareComboServiceStatus.REPROCESS);

                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = takecareComboOrderModel;
                    result.Message = "Cancel Takecare combo service order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Cancel Takecare combo service order failed.";
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

        public async Task<ResultModel> ChangeTakecareComboOrderStatus(Guid id, string status, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.TECHNICIAN)
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
                bool update = await _takecareComboOrderRepo.ChangeTakecareComboOrderStatus(id, status);
                if (update == true)
                {
                    if (status == Status.COMPLETED)
                    {
                        var comboServiceOrder = await _takecareComboOrderRepo.Get(id);
                        await _takecareComboServiceRepo.ChangeTakecareComboServiceStatus(comboServiceOrder.TakecareComboServiceId, Status.COMPLETED);

                        TblUser tblUser = await _userRepo.Get((Guid)comboServiceOrder.UserId);
                        ResultModel resultCareGuideGen = await GenerateComboServiceCareGuidePDF(comboServiceOrder.Id);
                        FileData fileCareGuide = (FileData)resultCareGuideGen.Data;
                        var checkMail = await _emailService.SendEmailComboServiceCareGuide(tblUser.Mail, comboServiceOrder.Id, fileCareGuide);


                        ResultModel careGuideURLResult = await _imageService.UploadAPDF(fileCareGuide);
                        var checkUpload = await _takecareComboServiceRepo.UpdateServiceOrderCareGuide(comboServiceOrder.Id, careGuideURLResult.Data.ToString());
                    }

                    TakecareComboOrderModel takecareComboOrderModel = await GetTakecareComboOrder(id);
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = takecareComboOrderModel;
                    result.Message = "Update Takecare combo service order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update Takecare combo service order failed.";
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

        public async Task<ResultModel> CreateTakecareComboOrder(TakecareComboOrderCreateModel takecareComboOrderCreateModel, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboOrderCreateModel.TakecareComboServiceId);
                TakecareComboServiceDetail takecareComboServiceDetail = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboService.Id);
                if (tblTakecareComboService != null)
                {
                    Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    double total = (double)takecareComboServiceDetail.TakecareComboPrice * tblTakecareComboService.NumberOfMonths * tblTakecareComboService.TreeQuantity;
                    double deposit = Math.Ceiling(total / 2);
                    TblTakecareComboOrder tblTakecareComboOrder = new()
                    {
                        Id = Guid.NewGuid(),
                        OrderCode = await GenerateOrderCode(),
                        CreateDate = currentTime,
                        UserId = userID,
                        ServiceStartDate = tblTakecareComboService.StartDate,
                        ServiceEndDate = tblTakecareComboService.EndDate,
                        Deposit = deposit,
                        TotalPrice = total,
                        RemainAmount = total,
                        TechnicianId = tblTakecareComboService.TechnicianId,
                        TakecareComboServiceId = tblTakecareComboService.Id,
                        Status = Status.UNPAID,
                    };
                    Guid insert = await _takecareComboOrderRepo.Insert(tblTakecareComboOrder);
                    if (insert != Guid.Empty)
                    {


                        var updateTakecareComboServiceStatus = await _takecareComboServiceRepo.ChangeTakecareComboServiceStatus(tblTakecareComboOrder.TakecareComboServiceId, TakecareComboServiceStatus.TAKINGCARE);
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = await GetTakecareComboOrder(tblTakecareComboOrder.Id);
                        result.Message = "Cretae Takecare combo service order success.";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Cretae Takecare combo service order failed.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare combo service ID invalid.";
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

        public async Task<ResultModel> GetAllTakcareComboOrder(PaginationRequestModel pagingModel, string status, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (userRole.Equals(Commons.CUSTOMER))
                {
                    string userID = _decodeToken.Decode(token, "userid");
                    var tblUser = await _userRepo.Get(Guid.Parse(userID));

                    Page<TblTakecareComboOrder> userTakecareComboOrders = await _takecareComboOrderRepo.GetAllTakecreComboOrderForCustomer(pagingModel, status, tblUser.Id);
                    if (userTakecareComboOrders != null)
                    {
                        List<TakecareComboOrderModel> takecareComboOrderModelList = new();
                        foreach (var item in userTakecareComboOrders.Results)
                        {
                            TakecareComboOrderModel takecareComboOrderModelAdd = await GetTakecareComboOrder(item.Id);
                            takecareComboOrderModelList.Add(takecareComboOrderModelAdd);
                        }
                        PaginationResponseModel paging = new PaginationResponseModel()
                            .PageSize(userTakecareComboOrders.PageSize)
                            .CurPage(userTakecareComboOrders.CurrentPage)
                            .RecordCount(userTakecareComboOrders.RecordCount)
                            .PageCount(userTakecareComboOrders.PageCount);
                        GetTakecareComboOrderResModel getTakecareComboOrderResModel = new()
                        {
                            Paging = paging,
                            TakecareComboOrderList = takecareComboOrderModelList
                        };
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Data = getTakecareComboOrderResModel;
                        result.Message = "Get Takecare combo service orders success.";
                        return result;
                    }
                }

                Page<TblTakecareComboOrder> takecareComboOrders = await _takecareComboOrderRepo.GetAllTakecreComboOrder(pagingModel, status);
                if (takecareComboOrders != null)
                {
                    List<TakecareComboOrderModel> takecareComboOrderModelList = new();
                    foreach (var item in takecareComboOrders.Results)
                    {
                        TakecareComboOrderModel takecareComboOrderModelAdd = await GetTakecareComboOrder(item.Id);
                        takecareComboOrderModelList.Add(takecareComboOrderModelAdd);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(takecareComboOrders.PageSize)
                        .CurPage(takecareComboOrders.CurrentPage)
                        .RecordCount(takecareComboOrders.RecordCount)
                        .PageCount(takecareComboOrders.PageCount);
                    GetTakecareComboOrderResModel getTakecareComboOrderResModel = new()
                    {
                        Paging = paging,
                        TakecareComboOrderList = takecareComboOrderModelList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = getTakecareComboOrderResModel;
                    result.Message = "Get Takecare combo service orders success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get Takecare combo service orders failed.";
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

        public async Task<ResultModel> GetAllTakcareComboOrderForTechnician(PaginationRequestModel pagingModel, TakecareComboOrderTechnicianReqModel model, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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
                Page<TblTakecareComboOrder> takecareComboOrders = await _takecareComboOrderRepo.GetAllTakecreComboOrderForTech(pagingModel, model);
                if (takecareComboOrders != null)
                {
                    List<TakecareComboOrderModel> takecareComboOrderModelList = new();
                    foreach (var item in takecareComboOrders.Results)
                    {
                        TakecareComboOrderModel takecareComboOrderModelAdd = await GetTakecareComboOrder(item.Id);
                        takecareComboOrderModelList.Add(takecareComboOrderModelAdd);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(takecareComboOrders.PageSize)
                        .CurPage(takecareComboOrders.CurrentPage)
                        .RecordCount(takecareComboOrders.RecordCount)
                        .PageCount(takecareComboOrders.PageCount);
                    GetTakecareComboOrderResModel getTakecareComboOrderResModel = new()
                    {
                        Paging = paging,
                        TakecareComboOrderList = takecareComboOrderModelList
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = getTakecareComboOrderResModel;
                    result.Message = "Get Takecare combo service orders success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get Takecare combo service orders failed.";
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

        public async Task<ResultModel> GetTakecareComboOrderByID(Guid takecareComboOdderID, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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

                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (userRole.Equals(Commons.CUSTOMER))
                {
                    string userID = _decodeToken.Decode(token, "userid");
                    var tblUser = await _userRepo.Get(Guid.Parse(userID));
                    TakecareComboOrderModel cutomerTakecareComboOrderModel = await GetTakecareComboOrder(takecareComboOdderID);
                    if (cutomerTakecareComboOrderModel != null)
                    {
                        if (cutomerTakecareComboOrderModel.TakecareComboService.UserId.Equals(tblUser.Id))
                        {
                            result.Code = 200;
                            result.IsSuccess = true;
                            result.Data = cutomerTakecareComboOrderModel;
                            result.Message = "Get Takecare combo service order success.";
                            return result;
                        }
                        else
                        {
                            result.Code = 401;
                            result.IsSuccess = false;
                            result.Message = "Unauthorized error.";
                            return result;
                        }
                    }
                }
                TakecareComboOrderModel takecareComboOrderModel = await GetTakecareComboOrder(takecareComboOdderID);
                if (takecareComboOrderModel != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = takecareComboOrderModel;
                    result.Message = "Get Takecare combo service order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get Takecare combo service order failed.";
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

        public async Task<ResultModel> GetTakecareComboOrderByOrderCode(string orderCode, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.TECHNICIAN)
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

                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                var takecareComboOrder = await _takecareComboOrderRepo.GetTakecareComboOrderByCode(orderCode);
                if (userRole.Equals(Commons.CUSTOMER))
                {
                    string userID = _decodeToken.Decode(token, "userid");
                    var tblUser = await _userRepo.Get(Guid.Parse(userID));
                    TakecareComboOrderModel cutomerTakecareComboOrderModel = await GetTakecareComboOrder(takecareComboOrder.Id);
                    if (cutomerTakecareComboOrderModel != null)
                    {
                        if (cutomerTakecareComboOrderModel.TakecareComboService.UserId.Equals(tblUser.Id))
                        {
                            result.Code = 200;
                            result.IsSuccess = true;
                            result.Data = cutomerTakecareComboOrderModel;
                            result.Message = "Get Takecare combo service order success.";
                            return result;
                        }
                        else
                        {
                            result.Code = 401;
                            result.IsSuccess = false;
                            result.Message = "Unauthorized error.";
                            return result;
                        }
                    }
                }
                TakecareComboOrderModel takecareComboOrderModel = await GetTakecareComboOrder(takecareComboOrder.Id);
                if (takecareComboOrderModel != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = takecareComboOrderModel;
                    result.Message = "Get Takecare combo service order success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get Takecare combo service order failed.";
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
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            string orderCode = "COMBO_ORD_" + currentTime.ToString("ddMMyyyy") + "_";
            bool dup = true;
            while (dup == true)
            {
                Random random = new();
                orderCode += new string(Enumerable.Repeat("0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray());
                bool checkCodeDup = await _takecareComboServiceRepo.CheckCodeDup(orderCode);
                dup = checkCodeDup != false;
            }

            return orderCode;
        }

        private async Task<TakecareComboOrderModel> GetTakecareComboOrder(Guid takecareComboOrderID)
        {
            try
            {
                TblTakecareComboOrder tblTakecareComboOrderGet = await _takecareComboOrderRepo.Get(takecareComboOrderID);
                TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboOrderGet.TakecareComboServiceId);
                TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                string nameCancelBy = null;
                try
                {
                    nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblTakecareComboOrderGet.CancelBy);
                }
                catch (Exception)
                {
                    nameCancelBy = null;
                }

                TakecareComboServiceViewModel takecareComboService = new()
                {
                    Id = tblTakecareComboServiceGet.Id,
                    Code = tblTakecareComboServiceGet.Code,
                    TakecareComboDetail = takecareComboServiceDetailGet ?? null,
                    CreateDate = tblTakecareComboServiceGet.CreateDate,
                    StartDate = tblTakecareComboServiceGet.StartDate,
                    EndDate = tblTakecareComboServiceGet.EndDate,
                    Name = tblTakecareComboServiceGet.Name,
                    Phone = tblTakecareComboServiceGet.Phone,
                    Email = tblTakecareComboServiceGet.Email,
                    Address = tblTakecareComboServiceGet.Address,
                    UserId = tblTakecareComboServiceGet.UserId,
                    TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                    TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "",
                    TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                    IsAtShop = (bool)tblTakecareComboServiceGet.IsAtShop,
                    NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
                    Status = tblTakecareComboServiceGet.Status,
                    CareGuide = tblTakecareComboServiceGet.CareGuide,
                    Reason = tblTakecareComboServiceGet.CancelReason
                };
                TakecareComboOrderModel takecareComboOrderModel = new()
                {
                    Id = tblTakecareComboOrderGet.Id,
                    OrderCode = tblTakecareComboOrderGet.OrderCode,
                    CreateDate = tblTakecareComboOrderGet.CreateDate,
                    ServiceStartDate = tblTakecareComboOrderGet.ServiceStartDate,
                    ServiceEndDate = tblTakecareComboOrderGet.ServiceEndDate,
                    Deposit = tblTakecareComboOrderGet.Deposit,
                    TotalPrice = tblTakecareComboOrderGet.TotalPrice,
                    RemainAmount = tblTakecareComboOrderGet.RemainAmount,
                    TechnicianId = tblTakecareComboOrderGet.TechnicianId,
                    UserId = tblTakecareComboOrderGet.UserId,
                    Status = tblTakecareComboOrderGet.Status,
                    Reason = tblTakecareComboOrderGet.Description,
                    CancelBy = tblTakecareComboOrderGet.CancelBy,
                    CareGuideURL = tblTakecareComboOrderGet.CareGuideUrl,
                    TakecareComboService = takecareComboService ?? null,
                    NameCancelBy = nameCancelBy,
                };
                return takecareComboOrderModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public async Task<ResultModel> GenerateComboServiceCareGuidePDF(Guid orderId)
        {
            var result = new ResultModel();
            try
            {
                var tblComboServiceOrder = await _takecareComboOrderRepo.Get(orderId);
                if (tblComboServiceOrder == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "OrderID invalid.";
                    return result;
                }
                var service = await _takecareComboServiceRepo.Get(tblComboServiceOrder.TakecareComboServiceId);


                var document = new PdfDocument();
                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";
                htmlContent += "<div style='width:100%; font: bold'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HƯỚNG DẪN CHĂM SÓC CÂY </h2>";

                if (!String.IsNullOrEmpty(service.CareGuide))
                {

                    htmlContent += "<h3> Hướng dẫn chăm sóc </h3>";


                    string a = service.CareGuide;
                    List<string> splitted = a.Split('.').ToList();

                    foreach (string b in splitted)
                    {
                        if (!b.Equals(splitted.Last()))
                        {
                            htmlContent += "<p>-" + b + ".</p>";
                        }

                    }
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

