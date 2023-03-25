using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ServiceDetailRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Repositories.UserTreeRepo;
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
        public TakecareService(IUserRepo userRepo, IImageService imageService, IUserTreeRepo userTreeRepo, IImageRepo imageRepo, IServiceRepo serviceRepo, IServiceDetailRepo serviceDetailRepo)
        {
            _decodeToken = new DecodeToken();
            _serviceRepo = serviceRepo;
            _serviceDetailRepo = serviceDetailRepo;
            _imageRepo = imageRepo;
            _userTreeRepo = userTreeRepo;
            _imageService = imageService;
            _userRepo = userRepo;
        }

        public async Task<ResultModel> AssignTechnician(string token, ServiceAssignModelManager serviceAssignModelManager)
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
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = resService.Id,
                        ServiceCode = resService.ServiceCode,
                        UserID = resService.UserId,
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
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Assign technician success.";
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
                bool checkUserTree = false;
                foreach (Guid id in serviceInsertModel.UserTreeIDList)
                {
                    TblUserTree tblUserTree = await _userTreeRepo.Get(id);
                    if (tblUserTree != null)
                    {
                        checkUserTree = false;
                    }
                    else
                    {
                        checkUserTree = true;
                    }

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
                TblService tblService = new TblService
                {
                    Id = Guid.NewGuid(),
                    ServiceCode = "SERVICE_" + await GenerateServiceCode(),
                    UserId = Guid.Parse(userID),
                    CreateDate = currentTime,
                    StartDate = serviceInsertModel.StartDate,
                    EndDate = serviceInsertModel.EndDate,
                    Name = serviceInsertModel.Name,
                    Phone = serviceInsertModel.Phone,
                    Email = serviceInsertModel.Email,
                    Address = serviceInsertModel.Address,
                    IsTransport = serviceInsertModel.IsTransport,
                    Status = ServiceStatus.PROCESSING,
                };
                Guid insert = await _serviceRepo.Insert(tblService);
                if (insert != Guid.Empty)
                {
                    foreach (Guid id in serviceInsertModel.UserTreeIDList)
                    {
                        TblUserTree tblUserTree = await _userTreeRepo.Get(id);
                        List<string> tblUserTreeImgs = await _imageRepo.GetImgUrlUserTree(tblUserTree.Id);
                        TblServiceDetail tblServiceDetail = new TblServiceDetail
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
                            List<string> serviceDetailImgs = new List<string>();
                            foreach (string imgUrl in tblUserTreeImgs)
                            {
                                string newImgUrl = await _imageService.ReUpload(imgUrl);
                                serviceDetailImgs.Add(newImgUrl);
                            }
                            foreach (string url in serviceDetailImgs)
                            {
                                TblImage tblImage = new TblImage
                                {
                                    ServiceDetailId = insertServiceDetail,
                                    ImageUrl = url
                                };
                                _ = await _imageRepo.Insert(tblImage);
                            }
                        }

                    }
                    TblService resService = await _serviceRepo.Get(tblService.Id);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = resService.Id,
                        ServiceCode = resService.ServiceCode,
                        UserID = resService.UserId,
                        CreateDate = resService.CreateDate ?? DateTime.MinValue,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        Status = resService.Status,
                        IsTransport = resService.IsTransport,
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
                List<TblService> tblServices = await _serviceRepo.GetAllRequest();
                List<ServiceResModel> resModels = new List<ServiceResModel>();
                foreach (TblService service in tblServices)
                {
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = service.Id,
                        ServiceCode = service.ServiceCode,
                        UserID = service.UserId,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        IsTransport = service.IsTransport,
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

        public async Task<ResultModel> GetUserRequest(string token)
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
                List<TblService> tblServices = await _serviceRepo.GetRequestByUser(Guid.Parse(userID));
                List<ServiceResModel> resModels = new List<ServiceResModel>();
                foreach (TblService service in tblServices)
                {
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = service.Id,
                        ServiceCode = service.ServiceCode,
                        UserID = service.UserId,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        Status = service.Status,
                        IsTransport = service.IsTransport,
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

        public async Task<ResultModel> UpdateRequestStatus(string token, ServiceStatusModel serviceStatusModel)
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
                    TblService resService = await _serviceRepo.Get(tblService.Id);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(resService.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = resService.Id,
                        ServiceCode = resService.ServiceCode,
                        UserID = resService.UserId,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Address = resService.Address,
                        Status = resService.Status,
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
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(getResService.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = getResService.Id,
                        ServiceCode = getResService.ServiceCode,
                        UserID = getResService.UserId,
                        CreateDate = getResService.CreateDate ?? DateTime.MinValue,
                        StartDate = getResService.StartDate,
                        EndDate = getResService.EndDate,
                        Name = getResService.Name,
                        Phone = getResService.Phone,
                        Email = getResService.Email,
                        Address = getResService.Address,
                        Status = getResService.Status,
                        IsTransport = getResService.IsTransport,
                        TechnicianID = getResService.TechnicianId,
                        TechnicianName = getResService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = serviceResModel;
                    result.Message = "Updated service with service detail changed.";
                }
                else
                {
                    TblService getResService = await _serviceRepo.Get(serviceUpdateModelManager.ServiceID);
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(getResService.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = getResService.Id,
                        UserID = getResService.UserId,
                        CreateDate = getResService.CreateDate ?? DateTime.MinValue,
                        StartDate = getResService.StartDate,
                        EndDate = getResService.EndDate,
                        Name = getResService.Name,
                        Phone = getResService.Phone,
                        Email = getResService.Email,
                        Address = getResService.Address,
                        Status = getResService.Status,
                        IsTransport = getResService.IsTransport,
                        TechnicianID = getResService.TechnicianId,
                        TechnicianName = getResService.TechnicianName,
                        ServiceDetailList = resServiceDetail
                    };
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
            string serviceCode = "";
            bool dup = true;
            while (dup == true)
            {
                Random random = new();
                serviceCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10).Select(s => s[random.Next(s.Length)]).ToArray());
                bool checkServiceCode = await _serviceRepo.CheckServiceCode(serviceCode);
                dup = checkServiceCode != false;
            }

            return serviceCode;
        }
    }
}

