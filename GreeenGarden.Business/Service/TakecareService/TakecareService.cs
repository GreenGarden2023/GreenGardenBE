using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using Newtonsoft.Json.Linq;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.ServiceDetailRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using GreeenGarden.Business.Service.ImageService;

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
        public TakecareService(IImageService imageService, IUserTreeRepo userTreeRepo,  IImageRepo imageRepo, IServiceRepo serviceRepo, IServiceDetailRepo serviceDetailRepo)
        {
            _decodeToken = new DecodeToken();
            _serviceRepo = serviceRepo;
            _serviceDetailRepo = serviceDetailRepo;
            _imageRepo = imageRepo;
            _userTreeRepo = userTreeRepo;
            _imageService = imageService;
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
                bool checkUserTree = false;
                foreach(Guid id in serviceInsertModel.UserTreeIDList)
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
                    UserId = Guid.Parse(userID),
                    CreateDate = currentTime,
                    StartDate = serviceInsertModel.StartDate,
                    EndDate = serviceInsertModel.EndDate,
                    Name = serviceInsertModel.Name,
                    Phone = serviceInsertModel.Phone,
                    Email = serviceInsertModel.Email,
                    Address = serviceInsertModel.Address,
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
                        UserID = resService.UserId,
                        CreateDate = resService.CreateDate ?? DateTime.MinValue,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Email = resService.Email,
                        Address = resService.Address,
                        Status = resService.Status,
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
                List<TblService> tblServices = await _serviceRepo.GetAllRequest();
                List<ServiceResModel> resModels = new List<ServiceResModel>();
                foreach (TblService service in tblServices)
                {
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = service.Id,
                        UserID = service.UserId,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        Status = service.Status,
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
                List<TblService> tblServices = await _serviceRepo.GetRequestByUser(Guid.Parse(userID));
                List<ServiceResModel> resModels = new List<ServiceResModel>();
                foreach (TblService service in tblServices)
                {
                    List<ServiceDetailResModel> resServiceDetail = await _serviceDetailRepo.GetServiceDetailByServiceID(service.Id);
                    ServiceResModel serviceResModel = new ServiceResModel
                    {
                        ID = service.Id,
                        UserID = service.UserId,
                        CreateDate = service.CreateDate ?? DateTime.MinValue,
                        StartDate = service.StartDate,
                        EndDate = service.EndDate,
                        Name = service.Name,
                        Phone = service.Phone,
                        Email = service.Email,
                        Address = service.Address,
                        Status = service.Status,
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
                        UserID = resService.UserId,
                        StartDate = resService.StartDate,
                        EndDate = resService.EndDate,
                        Name = resService.Name,
                        Phone = resService.Phone,
                        Address = resService.Address,
                        Status = resService.Status,
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
    }
}

