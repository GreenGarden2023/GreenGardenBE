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
using GreeenGarden.Data.Repositories.TakecareComboServiceDetailRepo;
using GreeenGarden.Data.Models.TakecareComboOrder;
using GreeenGarden.Business.Service.TakecareComboService;
using GreeenGarden.Data.Repositories.TakecareComboOrderRepo;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Business.Service.TakecareComboServiceServ
{
    public class TakecareComboServiceServ : ITakecareComboServiceServ
    {
        public readonly ITakecareComboServiceRepo _takecareComboServiceRepo;
        private readonly ITakecareComboRepo _takecareComboRepo;
        public readonly IUserRepo _userRepo;
        private readonly ITakecareComboServiceDetailRepo _takecareComboServiceDetailRepo;
        private readonly ITakecareComboOrderRepo _takecareComboOrderRepo;
        private readonly IEMailService _eMailService;

        private readonly DecodeToken _decodeToken;
        public TakecareComboServiceServ(ITakecareComboServiceRepo takecareComboServiceRepo, IUserRepo userRepo,
            ITakecareComboRepo takecareComboRepo, ITakecareComboServiceDetailRepo takecareComboServiceDetailRepo,
            ITakecareComboOrderRepo takecareComboOrderRepo, IEMailService eMailService)
        {
            _takecareComboServiceRepo = takecareComboServiceRepo;
            _decodeToken = new DecodeToken();
            _takecareComboRepo = takecareComboRepo;
            _userRepo = userRepo;
            _takecareComboServiceDetailRepo = takecareComboServiceDetailRepo;
            _takecareComboOrderRepo = takecareComboOrderRepo;
            _eMailService= eMailService;
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
                if (tblTakecareComboService == null)
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
                        TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                        TakecareComboServiceViewModel returnModel = new()
                        {
                            Id = tblTakecareComboServiceGet.Id,
                            Code = tblTakecareComboServiceGet.Code,
                            TakecareComboDetail = takecareComboServiceDetailGet,
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
                        };

                        var tblUser = await _userRepo.Get((Guid)tblTakecareComboServiceGet.TechnicianId);
                        _ = await _eMailService.SendEmailAssignTechnician(tblUser.Mail, tblTakecareComboServiceGet.Code);
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
                        TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                        TakecareComboServiceViewModel returnModel = new()
                        {
                            Id = tblTakecareComboServiceGet.Id,
                            Code = tblTakecareComboServiceGet.Code,
                            TakecareComboDetail = takecareComboServiceDetailGet,
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
                            CareGuide = tblTakecareComboServiceGet.CareGuide,
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

                TblTakecareCombo tblTakecareCombo = await _takecareComboRepo.Get(takecareComboServiceInsertModel.TakecareComboId);
                TblTakecareComboService tblTakecareComboService = new()
                {
                    Id = Guid.NewGuid(),
                    Code = await GenerateOrderCode(),
                    TakecareComboId = takecareComboServiceInsertModel.TakecareComboId,
                    CreateDate = currentTime,
                    IsAtShop = takecareComboServiceInsertModel.IsAtShop,
                    NumberOfMonths = takecareComboServiceInsertModel.NumOfMonth,
                    StartDate = test1,
                    EndDate = test2,
                    Name = takecareComboServiceInsertModel.Name,
                    Phone = takecareComboServiceInsertModel.Phone,
                    Email = takecareComboServiceInsertModel.Email,
                    Address = takecareComboServiceInsertModel.Address,
                    UserId = userID,
                    TreeQuantity = takecareComboServiceInsertModel.TreeQuantity,
                    CareGuide = tblTakecareCombo.Careguide,
                    Status = TakecareComboServiceStatus.PENDING,
                };
                Guid insert = await _takecareComboServiceRepo.Insert(tblTakecareComboService);
                if (insert != Guid.Empty)
                {
                    TblTakecareComboServiceDetail takecareComboServiceDetail = new()
                    {
                        Id = Guid.NewGuid(),
                        TakecareComboServiceId = tblTakecareComboService.Id,
                        TakecareComboId = takecareComboServiceInsertModel.TakecareComboId,
                        TakecareComboName = tblTakecareCombo.Name,
                        TakecareComboDescription = tblTakecareCombo.Description,
                        TakecareComboGuarantee = tblTakecareCombo.Guarantee,
                        TakecareComboCareguide= tblTakecareCombo.Careguide,
                        TakecareComboPrice = tblTakecareCombo.Price
                    };
                    Guid insertDetail = await _takecareComboServiceDetailRepo.Insert(takecareComboServiceDetail);
                    TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                    TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
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
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
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
                List<TakecareComboServiceViewModel> resList = new List<TakecareComboServiceViewModel>();
                if (userRole.Equals(Commons.CUSTOMER))
                {

                    string userID =  _decodeToken.Decode(token, "userid");
                    List<TblTakecareComboService> getList = await _takecareComboServiceRepo.GetAllTakecareComboServiceByCustomer(status, Guid.Parse(userID));
                    foreach (var item in getList)
                    {
                        TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(item.Id);
                        var order = await GetTakecareComboOrder(item.Id);
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)item.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
                        TakecareComboServiceViewModel returnModel = new()
                        {
                            Id = item.Id,
                            Code = item.Code,
                            TakecareComboDetail = takecareComboServiceDetailGet,
                            CreateDate = item.CreateDate,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            Name = item.Name,
                            Phone = item.Phone,
                            Email = item.Email,
                            Address = item.Address,
                            UserId = item.UserId,
                            TechnicianId = item.TechnicianId ?? Guid.Empty,
                            TechnicianName = item.TechnicianName ?? "",
                            TreeQuantity = item.TreeQuantity,
                            CareGuide = item.CareGuide,
                            IsAtShop = (bool)item.IsAtShop,
                            NumOfMonths = item.NumberOfMonths,
                            Status = item.Status,
                            takecareComboOrder = order,
                            CancelBy = item.CancelBy,
                            Reason = item.CancelReason,
                            NameCancelBy = nameCancelBy,
                        };
                        resList.Add(returnModel);

                    }
                }
                else
                {
                    List<TblTakecareComboService> getList = await _takecareComboServiceRepo.GetAllTakecareComboService(status);
                    foreach (var item in getList)
                    {
                        TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(item.Id);
                        var order = await GetTakecareComboOrder(item.Id);
                        string nameCancelBy = null;
                        try
                        {
                            nameCancelBy = await _userRepo.GetFullNameByID((Guid)item.CancelBy);
                        }
                        catch (Exception)
                        {
                            nameCancelBy = null;
                        }
                        TakecareComboServiceViewModel returnModel = new()
                        {
                            Id = item.Id,
                            Code = item.Code,
                            TakecareComboDetail = takecareComboServiceDetailGet,
                            CreateDate = item.CreateDate,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            Name = item.Name,
                            Phone = item.Phone,
                            Email = item.Email,
                            Address = item.Address,
                            UserId = item.UserId,
                            TechnicianId = item.TechnicianId ?? Guid.Empty,
                            TechnicianName = item.TechnicianName ?? "",
                            TreeQuantity = item.TreeQuantity,
                            CareGuide = item.CareGuide,
                            IsAtShop = (bool)item.IsAtShop,
                            NumOfMonths = item.NumberOfMonths,
                            Status = item.Status,
                            takecareComboOrder = order,
                            CancelBy = item.CancelBy,
                            Reason = item.CancelReason,
                            NameCancelBy = nameCancelBy,
                        };
                        resList.Add(returnModel);
                    }

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

        private async Task<TakecereComboOrderServiceResModel> GetTakecareComboOrder(Guid takecareServiceID)
        {
            try
            {
                TblTakecareComboOrder tblTakecareComboOrderGet = await _takecareComboServiceDetailRepo.getComboOrderByServiceID(takecareServiceID);
                if (tblTakecareComboOrderGet == null)
                {
                    return null;
                }
                string nameCancelBy = null;
                try
                {
                    nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblTakecareComboOrderGet.CancelBy);
                }
                catch (Exception)
                {
                    nameCancelBy = null;
                }
                TakecereComboOrderServiceResModel takecareComboOrderModel = new()
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
                TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                if (tblTakecareComboServiceGet != null)
                {
                    var order = await GetTakecareComboOrder(tblTakecareComboServiceGet.Id);
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
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
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        IsAtShop = (bool)tblTakecareComboServiceGet.IsAtShop,
                        NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
                        Status = tblTakecareComboServiceGet.Status,
                        takecareComboOrder = order,
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
                    result.Message = "Can not find takecare combo service with id " + takecareComboServiceId + ".";
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
                    TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
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
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        Status = tblTakecareComboServiceGet.Status,
                    };

                    TblUser user = await _userRepo.Get(tblTakecareComboServiceGet.UserId);
                    _ = await _eMailService.SendEmailComboServiceUpdate(user.Mail, tblTakecareComboServiceGet.Code);
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
            string orderCode = "COMBO_SERV_" + currentTime.ToString("ddMMyyyy") + "_";
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

        public async Task<ResultModel> CancelService(TakecareComboServiceCancelModel takecareComboServiceCancelModel, string token)
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
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboServiceCancelModel.TakecareComboServiceId);
                if (tblTakecareComboService == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare Combo Service Id invalid.";
                    return result;
                }
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                bool change = await _takecareComboServiceRepo.CancelService(takecareComboServiceCancelModel.TakecareComboServiceId, takecareComboServiceCancelModel.CancelReason, userID);
                if (change == true)
                {
                    TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                    TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblTakecareComboServiceGet.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
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
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
                        CancelBy = tblTakecareComboServiceGet.CancelBy,
                        NameCancelBy= nameCancelBy,
                        Reason = tblTakecareComboServiceGet.CancelReason,
                        Status = tblTakecareComboServiceGet.Status,
                    };
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = returnModel;
                    result.Message = "Cancel Takecare Combo Service status successful";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Cancel Takecare Combo Service status failed.";
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

        public async Task<ResultModel> GetAllTakecareComboServiceForTechnician(string? serviceCode, string status, string token, Guid technician)
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
                List<TblTakecareComboService> getList = await _takecareComboServiceRepo.GetAllTakecareComboServiceByTech(serviceCode,status, technician);
                List<TakecareComboServiceViewModel> resList = new List<TakecareComboServiceViewModel>();
                foreach (var item in getList)
                {
                    TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(item.Id);
                    var order = await GetTakecareComboOrder(item.Id);
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
                        CreateDate = item.CreateDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Name = item.Name,
                        Phone = item.Phone,
                        Email = item.Email,
                        Address = item.Address,
                        UserId = item.UserId,
                        CareGuide = item.CareGuide,
                        TechnicianId = item.TechnicianId ?? Guid.Empty,
                        TechnicianName = item.TechnicianName ?? "",
                        TreeQuantity = item.TreeQuantity,
                        IsAtShop = (bool)item.IsAtShop,
                        NumOfMonths = item.NumberOfMonths,
                        Status = item.Status,
                        takecareComboOrder = order
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

        public async Task<ResultModel> GetTakecareComboServiceByCode(string code, string token)
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
                TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.GetTakecareComboServiceByCode(code);
                TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                if (tblTakecareComboServiceGet != null)
                {
                    var order = await GetTakecareComboOrder(tblTakecareComboServiceGet.Id);
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
                        CreateDate = tblTakecareComboServiceGet.CreateDate,
                        StartDate = tblTakecareComboServiceGet.StartDate,
                        EndDate = tblTakecareComboServiceGet.EndDate,
                        Name = tblTakecareComboServiceGet.Name,
                        Phone = tblTakecareComboServiceGet.Phone,
                        Email = tblTakecareComboServiceGet.Email,
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        Address = tblTakecareComboServiceGet.Address,
                        UserId = tblTakecareComboServiceGet.UserId,
                        TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                        TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "",
                        TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                        IsAtShop = (bool)tblTakecareComboServiceGet.IsAtShop,
                        NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
                        Status = tblTakecareComboServiceGet.Status,
                        takecareComboOrder = order,
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
                    result.Message = "Can not find takecare combo service with code " + code + ".";
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

        public async Task<ResultModel> RejectService(TakecareComboServiceRejectModel takecareComboServiceRejectModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER) && !userRole.Equals(Commons.MANAGER) && !userRole.Equals(Commons.TECHNICIAN)&& !userRole.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                TblTakecareComboService tblTakecareComboService = await _takecareComboServiceRepo.Get(takecareComboServiceRejectModel.TakecareComboServiceId);
                if (tblTakecareComboService == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Takecare Combo Service Id invalid.";
                    return result;
                }
                Guid userID = Guid.Parse(_decodeToken.Decode(token, "userid"));
                bool change = await _takecareComboServiceRepo.RejectService(takecareComboServiceRejectModel.TakecareComboServiceId, takecareComboServiceRejectModel.RejectReason, userID);
                if (change == true)
                {
                    TblTakecareComboService tblTakecareComboServiceGet = await _takecareComboServiceRepo.Get(tblTakecareComboService.Id);
                    TakecareComboServiceDetail takecareComboServiceDetailGet = await _takecareComboServiceDetailRepo.GetTakecareComboServiceDetail(tblTakecareComboServiceGet.Id);

                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblTakecareComboServiceGet.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    TakecareComboServiceViewModel returnModel = new()
                    {
                        Id = tblTakecareComboServiceGet.Id,
                        Code = tblTakecareComboServiceGet.Code,
                        TakecareComboDetail = takecareComboServiceDetailGet,
                        CreateDate = tblTakecareComboServiceGet.CreateDate,
                        StartDate = tblTakecareComboServiceGet.StartDate,
                        EndDate = tblTakecareComboServiceGet.EndDate,
                        Name = tblTakecareComboServiceGet.Name,
                        Phone = tblTakecareComboServiceGet.Phone,
                        Email = tblTakecareComboServiceGet.Email,
                        CareGuide = tblTakecareComboServiceGet.CareGuide,
                        Address = tblTakecareComboServiceGet.Address,
                        UserId = tblTakecareComboServiceGet.UserId,
                        TechnicianId = tblTakecareComboServiceGet.TechnicianId ?? Guid.Empty,
                        TechnicianName = tblTakecareComboServiceGet.TechnicianName ?? "",
                        TreeQuantity = tblTakecareComboServiceGet.TreeQuantity,
                        IsAtShop = (bool)tblTakecareComboServiceGet.IsAtShop,
                        NumOfMonths = tblTakecareComboServiceGet.NumberOfMonths,
                        Status = tblTakecareComboServiceGet.Status,
                        Reason = tblTakecareComboServiceGet.CancelReason,
                        CancelBy = tblTakecareComboServiceGet.CancelBy,
                        NameCancelBy = nameCancelBy,

                    };
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = returnModel;
                    result.Message = "Cancel Takecare Combo Service status successful";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Cancel Takecare Combo Service status failed.";
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

        public async Task<ResultModel> GetTakecareComboServiceByPhone(GetServiceByPhoneModel model, PaginationRequestModel pagingModel, string token)
        {
            var result = new ResultModel();
            try
            {
                var listService = await _takecareComboServiceRepo.GetTakecareComboServiceByPhone(model, pagingModel);
                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listService.PageSize)
                    .CurPage(listService.CurrentPage)
                    .RecordCount(listService.RecordCount)
                    .PageCount(listService.PageCount);
                var res = new ServiceByPhoneResModel()
                {
                    Paging = paging,
                    TakecareComboServiceList = listService.Results
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = res;
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

