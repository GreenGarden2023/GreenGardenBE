using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.TakecareComboServiceRepo;
using Newtonsoft.Json.Linq;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Repositories.TakecareComboRepo;

namespace GreeenGarden.Business.Service.TakecareComboServiceServ
{
	public class TakecareComboServiceServ : ITakecareComboServiceServ
	{
        public readonly ITakecareComboServiceRepo _takecareComboServiceRepo;
        private readonly ITakecareComboRepo _takecareComboRepo;
        public readonly IUserRepo _userRepo;
        private readonly DecodeToken _decodeToken;
        public TakecareComboServiceServ(ITakecareComboServiceRepo takecareComboServiceRepo, IUserRepo userRepo, ITakecareComboRepo takecareComboRepo)
		{
            _takecareComboServiceRepo = takecareComboServiceRepo;
            _decodeToken = new DecodeToken();
            _takecareComboRepo = takecareComboRepo;
            _userRepo = userRepo;
        }

        public async Task<ResultModel> AssignTechnicianTakecareComboService(TakecareComboServiceAssignTechModel takecareComboServiceAssignTechModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboServiceAssignTechModel.TakecareComboServiceId);
                if (tblTakecareComboService == null )
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare Combo Service Id invalid.";
                    return result;
                }
                TblUser user = await _userRepo.Get(takecareComboServiceAssignTechModel.TechnicianID);
                if (user != null)
                {
                    bool assign = await _takecareComboServiceRepo.AssignTechnicianTakecareComboService(takecareComboServiceAssignTechModel.TakecareComboServiceId, takecareComboServiceAssignTechModel.TechnicianID);
                    if (assign == true)
                    {
                        TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                        TakecareComboServiceModel returnModel = new()
                        {
                            Id = tblTakecareComboServiceGet.Id,
                            Code = tblTakecareComboServiceGet.Code,
                            TakecareComboId = tblTakecareComboServiceGet.TakecareComboId,
                            CreateDate = tblTakecareComboServiceGet.CreateDate,
                            StartDate = tblTakecareComboServiceGet.StartDate,
                            EndDate = tblTakecareComboServiceGet.EndDate,
                            Name = tblTakecareComboServiceGet.Name,
                            Phone = tblTakecareComboServiceGet.Phone,
                            Email = tblTakecareComboServiceGet.Email,
                            Address = tblTakecareComboServiceGet.Address,
                            UserId = tblTakecareComboServiceGet.UserId,
                            TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                            TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "Not yet assign.",
                            TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                            Status = tblTakecareComboServiceGet.Status,
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = returnModel;
                        result.Message = "Assign technician successful";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Assign technician failed.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Technician Id invalid.";
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

        public async Task<ResultModel> ChangeTakecareComboServiceStatus(TakecareComboServiceChangeStatusModel takecareComboServiceChangeStatusModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboServiceChangeStatusModel.TakecareComboServiceId);
                if (tblTakecareComboService == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare Combo Service Id invalid.";
                    return result;
                }
                if (takecareComboServiceChangeStatusModel.Status.Trim().ToLower().Equals("accepted")
                    || takecareComboServiceChangeStatusModel.Status.Trim().ToLower().Equals("rejected")
                    || takecareComboServiceChangeStatusModel.Status.Trim().ToLower().Equals("pending"))
                {
                    bool change = await _takecareComboServiceRepo.ChangeTakecareComboServiceStatus(takecareComboServiceChangeStatusModel.TakecareComboServiceId, takecareComboServiceChangeStatusModel.Status);
                    if (change == true)
                    {
                        TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                        TakecareComboServiceModel returnModel = new()
                        {
                            Id = tblTakecareComboServiceGet.Id,
                            Code = tblTakecareComboServiceGet.Code,
                            TakecareComboId = tblTakecareComboServiceGet.TakecareComboId,
                            CreateDate = tblTakecareComboServiceGet.CreateDate,
                            StartDate = tblTakecareComboServiceGet.StartDate,
                            EndDate = tblTakecareComboServiceGet.EndDate,
                            Name = tblTakecareComboServiceGet.Name,
                            Phone = tblTakecareComboServiceGet.Phone,
                            Email = tblTakecareComboServiceGet.Email,
                            Address = tblTakecareComboServiceGet.Address,
                            UserId = tblTakecareComboServiceGet.UserId,
                            TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                            TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "Not yet assign.",
                            TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                            Status = tblTakecareComboServiceGet.Status,
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = returnModel;
                        result.Message = "Update Takecare Combo Service status successful";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Update Takecare Combo Service status failed.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Status invalid.";
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

        public async Task<ResultModel> CreateTakecareComboService(TakecareComboServiceInsertModel takecareComboServiceInsertModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                var test1 = DateTime.ParseExact(takecareComboServiceInsertModel.StartDate, "dd/MM/yyyy", null);
                var test2 = DateTime.ParseExact(takecareComboServiceInsertModel.StartDate, "dd/MM/yyyy", null).AddMonths(takecareComboServiceInsertModel.NumOfMonth);
                TblTakecareComboService tblTakecareComboService = new()
                {
                    Id = Guid.NewGuid(),
                    Code = await GenerateOrderCode(),
                    TakecareComboId = takecareComboServiceInsertModel.TakecareComboId,
                    CreateDate = currentTime,
                    IsAtShop = takecareComboServiceInsertModel.IsAtShop,
                    NumberOfMonths = takecareComboServiceInsertModel.NumOfMonth,
                    StartDate = DateTime.Parse(takecareComboServiceInsertModel.StartDate),
                    EndDate = DateTime.Parse(takecareComboServiceInsertModel.StartDate).AddMonths(takecareComboServiceInsertModel.NumOfMonth),
                    Name = takecareComboServiceInsertModel.Name,
                    Phone = takecareComboServiceInsertModel.Phone,
                    Email = takecareComboServiceInsertModel.Email,
                    Address = takecareComboServiceInsertModel.Address,
                    UserId = userID,
                    TreeQuantity = takecareComboServiceInsertModel.TreeQuantity,
                    Status = TakecareComboServiceStatus.PENDING,
                };
                Guid insert = await _takecareComboServiceRepo.Insert(tblTakecareComboService);
                if (insert != Guid.Empty)
                {
                    TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                    TakecareComboServiceModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboId = tblTakecareComboServiceGet.TakecareComboId,
                        CreateDate = tblTakecareComboServiceGet.CreateDate,
                        StartDate = tblTakecareComboServiceGet.StartDate,
                        EndDate = tblTakecareComboServiceGet.EndDate,
                        Name = tblTakecareComboServiceGet.Name,
                        Phone = tblTakecareComboServiceGet.Phone,
                        Email = tblTakecareComboServiceGet.Email,
                        Address = tblTakecareComboServiceGet.Address,
                        UserId = tblTakecareComboServiceGet.UserId,
                        TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                        TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "Not yet assign.",
                        TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                        Status = tblTakecareComboServiceGet.Status,
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = returnModel;
                    result.Message = "Create takecare combo service successful.";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create takecare combo service failed.";
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

        public async Task<ResultModel> GetAllTakecareComboService(string status, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                List<TblTakecareComboService> getList = await _takecareComboServiceRepo.GetAllTakecareComboService(status);
                List<TakecareComboServiceModel> resList = new List<TakecareComboServiceModel>();
                foreach (var item in getList)
                {
                    TakecareComboServiceModel returnModel = new()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        TakecareComboId = item.TakecareComboId,
                        CreateDate = item.CreateDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Name = item.Name,
                        Phone = item.Phone,
                        Email = item.Email,
                        Address = item.Address,
                        UserId = item.UserId,
                        TechnicianId = item.TechnicianId ?? Guid.Empty,
                        TechnicianName = item.TechnicianName ?? "Not yet assign.",
                        TreeQuantity = item.TreeQuantity,
                        Status = item.Status,
                    };
                    resList.Add(returnModel);
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = resList;
                result.Message = "Get takecare combo services successful.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetTakecareComboServiceByID(Guid takecareComboServiceId, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(takecareComboServiceId);
                if (tblTakecareComboServiceGet != null)
                {
                    TakecareComboServiceModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboId = tblTakecareComboServiceGet.TakecareComboId,
                        CreateDate = tblTakecareComboServiceGet.CreateDate,
                        StartDate = tblTakecareComboServiceGet.StartDate,
                        EndDate = tblTakecareComboServiceGet.EndDate,
                        Name = tblTakecareComboServiceGet.Name,
                        Phone = tblTakecareComboServiceGet.Phone,
                        Email = tblTakecareComboServiceGet.Email,
                        Address = tblTakecareComboServiceGet.Address,
                        UserId = tblTakecareComboServiceGet.UserId,
                        TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                        TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "Not yet assign.",
                        TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                        Status = tblTakecareComboServiceGet.Status,
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = returnModel;
                    result.Message = "Get takecare combo service successful.";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Can not find takecare combo service with id "+ takecareComboServiceId + ".";
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

        public async Task<ResultModel> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboServiceUpdateModel.Id);
                if (tblTakecareComboService == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare Combo Service Id invalid.";
                    return result;
                }
                if (takecareComboServiceUpdateModel.TakecareComboId != null)
                {
                    TblTakecareCombo tblTakecareComboGet = await _takecareComboRepo.Get((Guid)takecareComboServiceUpdateModel.TakecareComboId);
                    if (tblTakecareComboGet == null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Takecare Combo Id invalid.";
                        return result;
                    }
                }
                bool update = await _takecareComboServiceRepo.UpdateTakecareComboService(takecareComboServiceUpdateModel);
                    if (update == true)
                    {
                        TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                        TakecareComboServiceModel returnModel = new()
                        {
                            Id = tblTakecareComboServiceGet.Id,
                            Code = tblTakecareComboServiceGet.Code,
                            TakecareComboId = tblTakecareComboServiceGet.TakecareComboId,
                            CreateDate = tblTakecareComboServiceGet.CreateDate,
                            StartDate = tblTakecareComboServiceGet.StartDate,
                            EndDate = tblTakecareComboServiceGet.EndDate,
                            Name = tblTakecareComboServiceGet.Name,
                            Phone = tblTakecareComboServiceGet.Phone,
                            Email = tblTakecareComboServiceGet.Email,
                            Address = tblTakecareComboServiceGet.Address,
                            UserId = tblTakecareComboServiceGet.UserId,
                            TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                            TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "Not yet assign.",
                            TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                            Status = tblTakecareComboServiceGet.Status,
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = returnModel;
                        result.Message = "Update takecare combo service successful.";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Update takecare combo service failed.";
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
            string orderCode = "COMBO_"+currentTime.ToString("ddMMyyyy")+"_"; 
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
    }
}

