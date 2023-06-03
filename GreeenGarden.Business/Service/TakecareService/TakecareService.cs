using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ServiceDetailRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.TakecareService
{
    public class TakecareService : ITakecareService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IServiceRepo _serviceRepo;
        private readonly IServiceDetailRepo _serviceDetailRepo;
        private readonly IImageRepo _imageRepo;
        private readonly IImageService _imageService;
        private readonly IUserTreeRepo _userTreeRepo;
        private readonly IUserRepo _userRepo;
        private readonly IRewardRepo _rewardRepo;
        private readonly IServiceOrderRepo _serviceOrderRepo;
        private readonly IEMailService _emailService;
        private readonly IServiceCalendarRepo _serCalendarRepo;
        public TakecareService(IEMailService eMailService, IServiceCalendarRepo serCalendarRepo, IServiceOrderRepo serviceOrderRepo, IRewardRepo rewardRepo, IUserRepo userRepo, IImageService imageService, IUserTreeRepo userTreeRepo, IImageRepo imageRepo, IServiceRepo serviceRepo, IServiceDetailRepo serviceDetailRepo)
        {
            _decodeToken = new DecodeToken();
            _serviceRepo = serviceRepo;
            _serviceDetailRepo = serviceDetailRepo;
            _imageRepo = imageRepo;
            _userTreeRepo = userTreeRepo;
            _imageService = imageService;
            _userRepo = userRepo;
            _rewardRepo = rewardRepo;
            _serviceOrderRepo = serviceOrderRepo;
            _emailService = eMailService;
            _serCalendarRepo = serCalendarRepo; 
        }

        public async Task<ResultModel> AssignTechnician(string token, ServiceAssignModelManager serviceAssignModelManager)
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
                TblUser tblUser = await _userRepo.Get(serviceAssignModelManager.TechnicianID);
                if (tblUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Technician ID is invalid.";
                    return result;
                }
                bool assign = await _serviceRepo.AssignTechnician(serviceAssignModelManager);
                if (assign == true)
                {
                    TblService resService = await _serviceRepo.Get(serviceAssignModelManager.ServiceID);
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
                        Reason = resService.Reason,
                        CancelBy = resService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        DistrictID = resService.DistrictId,
                        Status = resService.Status,
                        IsTransport = resService.IsTransport,
                        TransportFee = resService.TransportFee,
                        RewardPointUsed = resService.RewardPointUsed,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };


                    _ = await _emailService.SendEmailAssignTechnician(tblUser.Mail, resService.ServiceCode);

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Đã chọn kỹ thuật viên "+ resService.TechnicianName + " chăm sóc.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Assign technician failed.";
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

        public async Task<ResultModel> CreateRequest(string token, ServiceInsertModel serviceInsertModel)
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
                string userID = _decodeToken.Decode(token, "userid");
                bool checkUserTree = false;
                foreach (Guid id in serviceInsertModel.UserTreeIDList)
                {
                    TblUserTree tblUserTree = await _userTreeRepo.Get(id);
                    checkUserTree = tblUserTree == null;

                }
                if (checkUserTree == true)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "A user tree ID is invalid.";
                    return result;
                }
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                TblService tblService = new()
                {
                    Id = Guid.NewGuid(),
                    ServiceCode = await GenerateServiceCode(),
                    UserId = Guid.Parse(userID),
                    CreateDate = currentTime,
                    StartDate = serviceInsertModel.StartDate,
                    EndDate = serviceInsertModel.EndDate,
                    Name = serviceInsertModel.Name,
                    Phone = serviceInsertModel.Phone,
                    Email = serviceInsertModel.Email,
                    Address = serviceInsertModel.Address,
                    DistrictId = serviceInsertModel.DistrictID,
                    IsTransport = serviceInsertModel.IsTransport,
                    TransportFee = 0,
                    RewardPointUsed = serviceInsertModel.RewardPointUsed,
                    Status = ServiceStatus.PROCESSING,
                };
                Guid insert = await _serviceRepo.Insert(tblService);
                if (insert != Guid.Empty)
                {
                    _ = await _rewardRepo.RemoveUserRewardPointByUserID(tblService.UserId, (int)tblService.RewardPointUsed);
                    foreach (Guid id in serviceInsertModel.UserTreeIDList)
                    {
                        TblUserTree tblUserTree = await _userTreeRepo.Get(id);
                        List<string> tblUserTreeImgs = await _imageRepo.GetImgUrlUserTree(tblUserTree.Id);
                        TblServiceDetail tblServiceDetail = new()
                        {
                            Id = Guid.NewGuid(),
                            UserTreeId = tblUserTree.Id,
                            ServiceId = tblService.Id,
                            TreeName = tblUserTree.TreeName,
                            Desciption = tblUserTree.Description,
                            Quantity = tblUserTree.Quantity,
                            ServicePrice = 0
                        };
                        Guid insertServiceDetail = await _serviceDetailRepo.Insert(tblServiceDetail);
                        if (insertServiceDetail != Guid.Empty)
                        {
                            List<string> serviceDetailImgs = new();
                            foreach (string imgUrl in tblUserTreeImgs)
                            {
                                string newImgUrl = await _imageService.ReUpload(imgUrl);
                                serviceDetailImgs.Add(newImgUrl);
                            }
                            foreach (string url in serviceDetailImgs)
                            {
                                TblImage tblImage = new()
                                {
                                    ServiceDetailId = insertServiceDetail,
                                    ImageUrl = url
                                };
                                _ = await _imageRepo.Insert(tblImage);
                            }
                        }

                    }
                    TblService resService = await _serviceRepo.Get(tblService.Id);
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
                        Reason = resService.Reason,
                        CancelBy = resService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        DistrictID = resService.DistrictId,
                        Status = resService.Status,
                        IsTransport = resService.IsTransport,
                        TransportFee = resService.TransportFee,
                        RewardPointUsed = resService.RewardPointUsed,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Create request success.";
                    return result;

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create request failed.";
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

        public async Task<ResultModel> GetAllRequest(string token)
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
                List<TblService> tblServices = await _serviceRepo.GetAllRequest();
                List<ServiceResModel> resModels = new();
                foreach (TblService service in tblServices)
                {
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)service.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    int userCurrentPoint = await _rewardRepo.GetUserRewardPoint(service.UserId);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = service.Id,
                        ServiceCode = service.ServiceCode,
                        UserId = service.UserId,
                        Rules = service.Rules,
                        Reason = service.Reason,
                        CancelBy = service.CancelBy,
                        NameCancelBy = nameCancelBy,
                        UserCurrentPoint = userCurrentPoint,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        DistrictID = service.DistrictId,
                        IsTransport = service.IsTransport,
                        TransportFee = service.TransportFee,
                        RewardPointUsed = service.RewardPointUsed,
                        Status = service.Status,
                        TechnicianID = service.TechnicianId,
                        TechnicianName = service.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    resModels.Add(serviceResModel);
                }
                resModels.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = resModels;
                result.Message = "Get request success.";
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

        public async Task<ResultModel> GetARequestDetail(string token, Guid serviceID)
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
                Guid orderID = Guid.Empty;
                TblServiceOrder tblServiceOrder = await _serviceOrderRepo.GetServiceOrderByServiceID(serviceID);
                if (tblServiceOrder != null)
                {
                    orderID = tblServiceOrder.Id;
                }
                TblService tblService = await _serviceRepo.Get(serviceID);
                string nameCancelBy = null;
                try
                {
                    nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblService.CancelBy);
                }
                catch (Exception)
                {
                    nameCancelBy = null;
                }

                List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(serviceID);
                ServiceResModel serviceResModel = new()
                {
                    ID = tblService.Id,
                    ServiceCode = tblService.ServiceCode,
                    UserId = tblService.UserId,
                    Rules = tblService.Rules,
                    ServiceOrderID = orderID,
                    Reason = tblService.Reason,
                    CancelBy = tblService.CancelBy,
                    NameCancelBy = nameCancelBy,
                    UserCurrentPoint = 0,
                    CreateDate = tblService.CreateDate ?? DateTime.MinValue,
                    StartDate = tblService.StartDate,
                    EndDate = tblService.EndDate,
                    Name = tblService.Name,
                    Phone = tblService.Phone,
                    Email = tblService.Email,
                    Address = tblService.Address,
                    DistrictID = tblService.DistrictId,
                    Status = tblService.Status,
                    IsTransport = tblService.IsTransport,
                    TransportFee = tblService.TransportFee,
                    RewardPointUsed = tblService.RewardPointUsed,
                    TechnicianID = tblService.TechnicianId,
                    TechnicianName = tblService.TechnicianName,
                    ServiceDetailList = resServiceDetail
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = serviceResModel;
                result.Message = "Get request success.";
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


        public async Task<ResultModel> GetUserRequest(string token)
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
                string userID = _decodeToken.Decode(token, "userid");
                List<TblService> tblServices = await _serviceRepo.GetRequestByUser(Guid.Parse(userID));

                List<ServiceResModel> resModels = new();
                foreach (TblService service in tblServices)
                {
                    Guid orderID = Guid.Empty;
                    TblServiceOrder tblServiceOrder = await _serviceOrderRepo.GetServiceOrderByServiceID(service.Id);
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)service.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    if (tblServiceOrder != null)
                    {
                        orderID = tblServiceOrder.Id;
                    }
                    int userCurrentPoint = await _rewardRepo.GetUserRewardPoint(service.UserId);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = service.Id,
                        ServiceCode = service.ServiceCode,
                        UserId = service.UserId,
                        Rules = service.Rules,
                        ServiceOrderID = orderID,
                        Reason = service.Reason,
                        CancelBy = service.CancelBy,
                        NameCancelBy = nameCancelBy,
                        UserCurrentPoint = userCurrentPoint,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        DistrictID = service.DistrictId,
                        Status = service.Status,
                        IsTransport = service.IsTransport,
                        TransportFee = service.TransportFee,
                        RewardPointUsed = service.RewardPointUsed,
                        TechnicianID = service.TechnicianId,
                        TechnicianName = service.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    resModels.Add(serviceResModel);
                }
                resModels.Sort((x, y) => y.CreateDate.CompareTo(x.CreateDate));

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = resModels;
                result.Message = "Get request success.";
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

        public async Task<ResultModel> GetRequestOrderByTechnician(string token, PaginationRequestModel pagingModel, Guid technicianID)
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
                var listRes = new List<ServiceByTechResModel>();
                var listTblService = await _serCalendarRepo.GetServiceByTechnician(pagingModel, technicianID);
                foreach (var i in listTblService.Results)
                {
                    int userCurrentPoint = await _rewardRepo.GetUserRewardPoint(i.UserId);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(i.Id);
                    var tblUser = await _userRepo.Get(i.UserId);
                    UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(tblUser.UserName);
                    TblUser technicianGet = await _userRepo.Get((Guid)i.TechnicianId);
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)i.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };

                    var res = new ServiceByTechResModel()
                    {
                        ID = i.Id,
                        ServiceCode = i.ServiceCode,
                        StartDate = (DateTime)i.StartDate,
                        EndDate = (DateTime)i.EndDate,
                        UserCurrentPoint = userCurrentPoint,
                        Name = i.Name,
                        Phone = i.Phone,
                        Email = i.Email,
                        Address = i.Address,
                        Status = i.Status,
                        DistrictID = (int)i.DistrictId,
                        User = userCurrResModel,
                        CreateDate = (DateTime)i.CreateDate,
                        TechnicianName = i.TechnicianName,
                        IsTransport = (bool)i.IsTransport,
                        TransportFee = (double)i.TransportFee,
                        Rules = i.Rules,
                        CancelBy = i.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Reason = i.Reason,
                        RewardPointUsed = (int)i.RewardPointUsed,
                        Technician = technicianRes,
                        ServiceDetailList = resServiceDetail,
                    };
                    listRes.Add(res);

                }
                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listTblService.PageSize)
                    .CurPage(listTblService.CurrentPage)
                    .RecordCount(listTblService.RecordCount)
                    .PageCount(listTblService.PageCount);

                var newRequest = new RequestListRes()
                {
                    Paging = paging,
                    RequestList = listRes
                };



                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newRequest;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateRequestStatus(string token, ServiceStatusModel serviceStatusModel)
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
                TblService tblService = await _serviceRepo.Get(serviceStatusModel.ServiceID);
                if (tblService == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Service ID invalid.";
                    return result;
                }
                bool update = await _serviceRepo.ChangeServiceStatus(serviceStatusModel.ServiceID, serviceStatusModel.status);
                if (update == true)
                {
                    if (serviceStatusModel.status == ServiceStatus.REJECTED)
                    {
                        await _rewardRepo.AddUserRewardPointByUserID(tblService.UserId, (int)tblService.RewardPointUsed);
                    }
                    TblService resService = await _serviceRepo.Get(tblService.Id);
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
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Reason = resService.Reason,
                        CancelBy = resService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Phone = resService.Phone,
                        Address = resService.Address,
                        DistrictID = resService.DistrictId,
                        Status = resService.Status,
                        IsTransport = resService.IsTransport,
                        TransportFee = resService.TransportFee,
                        RewardPointUsed = resService.RewardPointUsed,
                        TechnicianID = resService.TechnicianId,
                        TechnicianName = resService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Update request status success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Update request status failed.";
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

        public async Task<ResultModel> UpdateServicePrice(string token, ServiceUpdateModelManager? serviceUpdateModelManager, List<ServiceDetailUpdateModelManager>? serviceDetailUpdateModelManagers)
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
                if (serviceUpdateModelManager != null)
                {
                    bool updateService = await _serviceRepo.UpdateServiceUserInfo(serviceUpdateModelManager);
                    if (updateService == false)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Update service failed.";
                        return result;
                    }

                }
                if (serviceDetailUpdateModelManagers != null)
                {
                    Guid serviceID = Guid.NewGuid();
                    foreach (ServiceDetailUpdateModelManager serviceDetail in serviceDetailUpdateModelManagers)
                    {
                        TblServiceDetail tblServiceDetail = await _serviceDetailRepo.Get(serviceDetail.ServiceDetailID);
                        if (tblServiceDetail == null)
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Service detail Id invalid.";
                            return result;
                        }
                        bool update = await _serviceDetailRepo.UpdateServiceDetailManager(serviceDetail);
                        serviceID = (Guid)tblServiceDetail.ServiceId;
                        if (update == false)
                        {
                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Update service detail failed.";
                            return result;
                        }
                    }

                    TblService getResService = await _serviceRepo.Get(serviceID);
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)getResService.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(getResService.Id);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = getResService.Id,
                        ServiceCode = getResService.ServiceCode,
                        UserId = getResService.UserId,
                        Rules = getResService.Rules,
                        CreateDate = getResService.CreateDate ?? DateTime.MinValue,
                        StartDate = getResService.StartDate,
                        EndDate = getResService.EndDate,
                        Name = getResService.Name,
                        Phone = getResService.Phone,
                        Email = getResService.Email,
                        Address = getResService.Address,
                        Reason = getResService.Reason,
                        CancelBy = getResService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        DistrictID = getResService.DistrictId,
                        Status = getResService.Status,
                        IsTransport = getResService.IsTransport,
                        TransportFee = getResService.TransportFee,
                        RewardPointUsed = getResService.RewardPointUsed,
                        TechnicianID = getResService.TechnicianId,
                        TechnicianName = getResService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    TblUser user = await _userRepo.Get(serviceResModel.UserId);
                    _ = await _emailService.SendEmailServiceUpdate(user.Mail, serviceResModel.ServiceCode);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Updated service with service detail changed.";
                }
                else
                {
                    TblService getResService = await _serviceRepo.Get(serviceUpdateModelManager.ServiceID);
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)getResService.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(getResService.Id);
                    ServiceResModel serviceResModel = new()
                    {
                        ID = getResService.Id,
                        UserId = getResService.UserId,
                        Rules = getResService.Rules,
                        ServiceCode = getResService.ServiceCode,
                        CreateDate = getResService.CreateDate ?? DateTime.MinValue,
                        StartDate = getResService.StartDate,
                        EndDate = getResService.EndDate,
                        Name = getResService.Name,
                        Phone = getResService.Phone,
                        Email = getResService.Email,
                        Address = getResService.Address,
                        DistrictID = getResService.DistrictId,
                        Status = getResService.Status,
                        Reason = getResService.Reason,
                        CancelBy = getResService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        IsTransport = getResService.IsTransport,
                        TechnicianID = getResService.TechnicianId,
                        TechnicianName = getResService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    TblUser user = await _userRepo.Get(serviceResModel.UserId);
                    _ = await _emailService.SendEmailServiceUpdate(user.Mail, serviceResModel.ServiceCode);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Updated service with out service detail change.";
                    return result;
                }

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

        private async Task<string> GenerateServiceCode()
        {


            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            string orderCode = "SERV_" + currentTime.ToString("ddMMyyyy") + "_";
            bool dup = true;
            while (dup == true)
            {
                Random random = new();
                orderCode += new string(Enumerable.Repeat("0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray());
                bool checkCodeDup = await _serviceRepo.CheckServiceCode(orderCode);
                dup = checkCodeDup != false;
            }

            return orderCode;

        }
        public async Task<ResultModel> GetRequestDetailByServiceOrder(string token, string serviceOrder)
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

                Guid orderID = Guid.Empty;
                TblServiceOrder tblServiceOrder = await _serviceOrderRepo.GetServiceOrderByOrderCode(serviceOrder);
                if (tblServiceOrder != null)
                {
                    orderID = tblServiceOrder.Id;
                }
                TblService tblService = await _serviceRepo.GetServiceByServiceCode(serviceOrder);
                string nameCancelBy = null;
                try
                {
                    nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblService.CancelBy);
                }
                catch (Exception)
                {
                    nameCancelBy = null;
                }
                if (tblService == null)
                {
                    result.Message = "Service not found.";
                    result.IsSuccess = false;
                    return result;
                }
                List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(tblService.Id);
                ServiceResModel serviceResModel = new()
                {
                    ID = tblService.Id,
                    ServiceCode = tblService.ServiceCode,
                    UserId = tblService.UserId,
                    Rules = tblService.Rules,
                    ServiceOrderID = orderID,
                    UserCurrentPoint = 0,
                    CreateDate = tblService.CreateDate ?? DateTime.MinValue,
                    StartDate = tblService.StartDate,
                    EndDate = tblService.EndDate,
                    Name = tblService.Name,
                    Phone = tblService.Phone,
                    Email = tblService.Email,
                    Address = tblService.Address,
                    DistrictID = tblService.DistrictId,
                    Status = tblService.Status,
                    Reason = tblService.Reason,
                    CancelBy = tblService.CancelBy,
                    NameCancelBy = nameCancelBy,
                    IsTransport = tblService.IsTransport,
                    TransportFee = tblService.TransportFee,
                    RewardPointUsed = tblService.RewardPointUsed,
                    TechnicianID = tblService.TechnicianId,
                    TechnicianName = tblService.TechnicianName,
                    ServiceDetailList = resServiceDetail
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = serviceResModel;
                result.Message = "Get request success.";
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

        public async Task<ResultModel> CancelRequest(string token, CancelRequestModel model)
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
                string userName = _decodeToken.Decode(token, "username");
                var user = await _userRepo.GetCurrentUser(userName);

                var service = await _serviceRepo.Get(model.serviceID);
                if (service ==  null)
                {
                    result.IsSuccess = false;
                    result.Message = "Không tìm thấy yêu cầu";
                    return result;
                }
                service.Status = ServiceStatus.CANCEL;
                service.CancelBy = user.Id;
                service.Reason = model.reason;
                await _rewardRepo.AddUserRewardPointByUserID(service.UserId,(int) service.RewardPointUsed);
                await _serviceRepo.UpdateService(service);

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cập nhật trạng thái thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRequestOrderByServiceCode(string token, PaginationRequestModel pagingModel, ServiceSearchByCodeModel model)
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
                var listRes = new List<ServiceByTechResModel>();
                var tblService = await _serviceRepo.GetServiceByServiceCode(model);
                    int userCurrentPoint = await _rewardRepo.GetUserRewardPoint(tblService.UserId);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(tblService.Id);
                    var tblUser = await _userRepo.Get(tblService.UserId);
                    UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(tblUser.UserName);
                    TblUser technicianGet = await _userRepo.Get((Guid)tblService.TechnicianId);
                    string nameCancelBy = null;
                    try
                    {
                        nameCancelBy = await _userRepo.GetFullNameByID((Guid)tblService.CancelBy);
                    }
                    catch (Exception)
                    {
                        nameCancelBy = null;
                    }
                    ServiceOrderTechnician technicianRes = new()
                    {
                        TechnicianID = technicianGet.Id,
                        TechnicianUserName = technicianGet.UserName,
                        TechnicianFullName = technicianGet.FullName,
                        TechnicianAddress = technicianGet.Address,
                        TechnicianMail = technicianGet.Mail,
                        TechnicianPhone = technicianGet.Phone
                    };

                    var res = new ServiceByTechResModel()
                    {
                        ID = tblService.Id,
                        ServiceCode = tblService    .ServiceCode,
                        StartDate = (DateTime)tblService.StartDate,
                        EndDate = (DateTime)tblService.EndDate,
                        UserCurrentPoint = userCurrentPoint,
                        Name = tblService.Name,
                        Phone = tblService.Phone,
                        Email = tblService.Email,
                        Address = tblService.Address,
                        Status = tblService.Status,
                        DistrictID = (int)tblService.DistrictId,
                        User = userCurrResModel,
                        CreateDate = (DateTime)tblService.CreateDate,
                        TechnicianName = tblService.TechnicianName,
                        IsTransport = (bool)tblService.IsTransport,
                        TransportFee = (double)tblService.TransportFee,
                        Rules = tblService.Rules,
                        CancelBy = tblService.CancelBy,
                        NameCancelBy = nameCancelBy,
                        Reason = tblService.Reason,
                        RewardPointUsed = (int)tblService.RewardPointUsed,
                        Technician = technicianRes,
                        ServiceDetailList = resServiceDetail,
                    };
                    listRes.Add(res);


                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(pagingModel.pageSize)
                    .CurPage(1)
                    .RecordCount(1)
                    .PageCount(1);

                var newRequest = new RequestListRes()
                {
                    Paging = paging,
                    RequestList = listRes
                };

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newRequest;
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

