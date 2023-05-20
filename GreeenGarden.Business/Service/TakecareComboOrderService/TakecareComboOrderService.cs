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

namespace GreeenGarden.Business.Service.TakecareComboOrderService
{
	public class TakecareComboOrderService : ITakecareComboOrderService
	{
		private readonly ITakecareComboOrderRepo _takecareComboOrderRepo;
		private readonly ITakecareComboServiceRepo _takecareComboServiceRepo;
        private readonly ITakecareComboServiceDetailRepo _takecareComboServiceDetailRepo;
        private readonly DecodeToken _decodeToken;
        public TakecareComboOrderService(ITakecareComboOrderRepo takecareComboOrderRepo, ITakecareComboServiceRepo takecareComboServiceRepo, ITakecareComboServiceDetailRepo takecareComboServiceDetailRepo)
		{
			_takecareComboOrderRepo = takecareComboOrderRepo;
			_takecareComboServiceRepo = takecareComboServiceRepo;
            _decodeToken = new DecodeToken();
            _takecareComboServiceDetailRepo = takecareComboServiceDetailRepo;
        }

        public async Task<ResultModel> ChangeTakecareComboOrderStatus(Guid id, string status, string token)
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
                bool update = await _takecareComboOrderRepo.ChangeTakecareComboOrderStatus(id, status);
                if (update != true)
                {
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

        public async Task<ResultModel> GetAllTakcareComboOrder(PaginationRequestModel pagingModel,string status, string token)
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

        public async Task<ResultModel> GetTakecareComboOrderByID(Guid takecareComboOdderID, string token)
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
                TakecareComboOrderModel takecareComboOrderModel = await GetTakecareComboOrder(takecareComboOdderID);
                if (takecareComboOrderModel != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data =  takecareComboOrderModel;
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
                    Description = tblTakecareComboOrderGet.Description,
                    CancelBy = tblTakecareComboOrderGet.CancelBy,
                    TakecareComboService = takecareComboService ?? null,
                };
                return takecareComboOrderModel;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        } 
    }
}

