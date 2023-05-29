using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.ServiceCalendarService
{
    public class ServiceCalendarService : IServiceCalendarService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IServiceCalendarRepo _serviceCalendarRepo;
        private readonly IImageRepo _imageRepo;
        private readonly IServiceRepo _serviceRepo;
        private readonly IServiceOrderRepo _serviceOrderRepo;
        private readonly IEMailService _emailService;
        private readonly IUserRepo _userRepo;
        public ServiceCalendarService(IUserRepo userRepo, IServiceCalendarRepo serviceCalendarRepo, IImageRepo imageRepo, IServiceRepo serviceRepo, IServiceOrderRepo serviceOrderRepo, IEMailService eMailService)
        {
            _decodeToken = new DecodeToken();
            _serviceCalendarRepo = serviceCalendarRepo;
            _imageRepo = imageRepo;
            _serviceRepo = serviceRepo;
            _serviceOrderRepo = serviceOrderRepo;
            _emailService = eMailService;
            _userRepo = userRepo;
        }

        public async Task<ResultModel> CreateServiceCalendar(string token, ServiceCalendarInsertModel serviceCalendarInsertModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
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
                if (serviceCalendarInsertModel.CalendarInitial != null && serviceCalendarInsertModel.CalendarUpdate == null)
                {
                    TblServiceCalendar tblServiceCalendar = new()
                    {
                        Id = Guid.NewGuid(),
                        ServiceOrderId = serviceCalendarInsertModel.CalendarInitial.ServiceOrderId,
                        ServiceDate = serviceCalendarInsertModel.CalendarInitial.ServiceDate,
                        Status = ServiceCalendarStatus.PENDING
                    };
                    Guid insert = await _serviceCalendarRepo.Insert(tblServiceCalendar);
                    if (insert != Guid.Empty)
                    {
                        TblServiceCalendar resGetModel = await _serviceCalendarRepo.Get(tblServiceCalendar.Id);
                        List<string> ImgUrls = await _imageRepo.GetImgUrlServiceCalendar(resGetModel.Id);
                        ServiceCalendarResModel resModel = new()
                        {
                            Id = resGetModel.Id,
                            ServiceOrderId = resGetModel.ServiceOrderId,
                            ServiceDate = resGetModel.ServiceDate,
                            NextServiceDate = resGetModel.NextServiceDate,
                            Sumary = resGetModel.Sumary,
                            Status = resGetModel.Status,
                            Images = ImgUrls
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = resModel;
                        result.Message = "Create calendar success";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Create calendar failed";
                        return result;
                    }
                }
                else if (serviceCalendarInsertModel.CalendarInitial == null && serviceCalendarInsertModel.CalendarUpdate != null)
                {
                    bool update = await _serviceCalendarRepo.UpdateServiceCalendar(serviceCalendarInsertModel.CalendarUpdate);
                    if (update == true)
                    {
                        TblServiceCalendar oldCalendarGet = await _serviceCalendarRepo.Get(serviceCalendarInsertModel.CalendarUpdate.ServiceCalendarId);
                        bool updateImg = await _imageRepo.UpdateImgForServiceCalendar(oldCalendarGet.Id, serviceCalendarInsertModel.CalendarUpdate.Images);
                        List<string> ImgUrls = await _imageRepo.GetImgUrlServiceCalendar(oldCalendarGet.Id);
                        ServiceCalendarResModel oldCalendarRes = new()
                        {
                            Id = oldCalendarGet.Id,
                            ServiceOrderId = oldCalendarGet.ServiceOrderId,
                            ServiceDate = oldCalendarGet.ServiceDate,
                            NextServiceDate = oldCalendarGet.NextServiceDate,
                            Sumary = oldCalendarGet.Sumary,
                            Status = oldCalendarGet.Status,
                            Images = ImgUrls
                        };

                        if (serviceCalendarInsertModel.CalendarUpdate.NextServiceDate != null)
                        {
                            TblServiceCalendar tblServiceCalendar = new()
                            {
                                Id = Guid.NewGuid(),
                                ServiceOrderId = oldCalendarRes.ServiceOrderId,
                                ServiceDate = serviceCalendarInsertModel.CalendarUpdate.NextServiceDate,
                                Status = ServiceCalendarStatus.PENDING
                            };
                            Guid insert = await _serviceCalendarRepo.Insert(tblServiceCalendar);
                            if (insert != Guid.Empty)
                            {

                                TblServiceCalendar nextCalendarGet = await _serviceCalendarRepo.Get(tblServiceCalendar.Id);
                                List<string> nextCaImgUrls = await _imageRepo.GetImgUrlServiceCalendar(nextCalendarGet.Id);
                                ServiceCalendarResModel nextCalendarRes = new()
                                {
                                    Id = nextCalendarGet.Id,
                                    ServiceOrderId = nextCalendarGet.ServiceOrderId,
                                    ServiceDate = nextCalendarGet.ServiceDate,
                                    NextServiceDate = nextCalendarGet.NextServiceDate,
                                    Sumary = nextCalendarGet.Sumary,
                                    Status = nextCalendarGet.Status,
                                    Images = nextCaImgUrls
                                };
                                ServiceCalendarUpdateResModel serviceCalendarUpdateResModel = new()
                                {
                                    PreviousCalendar = oldCalendarRes,
                                    NextCalendar = nextCalendarRes
                                };
                                TblServiceOrder tblServiceOrder = await _serviceOrderRepo.Get(oldCalendarRes.ServiceOrderId);
                                TblUser tblUser = await _userRepo.Get(tblServiceOrder.UserId);
                                _ = await _emailService.SendEmailReportUpdate(tblUser.Mail, oldCalendarRes.Id);
                                result.Code = 200;
                                result.IsSuccess = true;
                                result.Data = serviceCalendarUpdateResModel;
                                result.Message = "Update calendar success";
                                return result;
                            }
                            else
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.Message = "Create next calendar failed";
                                return result;
                            }
                        }
                        else
                        {

                            //bool updateOrder = await _serviceOrderRepo.CompleteServiceOrder(oldCalendarRes.ServiceOrderId);
                            TblServiceOrder tblServiceOrder = await _serviceOrderRepo.Get(oldCalendarRes.ServiceOrderId);
                            //bool updateService = await _serviceRepo.ChangeServiceStatus(tblServiceOrder.ServiceId, ServiceStatus.COMPLETED);
                            ServiceCalendarUpdateResModel serviceCalendarUpdateResModel = new()
                            {
                                PreviousCalendar = oldCalendarRes,
                                NextCalendar = null
                            };
                            TblUser tblUser = await _userRepo.Get(tblServiceOrder.UserId);
                            _ = await _emailService.SendEmailReportUpdate(tblUser.Mail, oldCalendarRes.Id);
                            result.Code = 200;
                            result.IsSuccess = true;
                            result.Data = serviceCalendarUpdateResModel;
                            result.Message = "Update calendar success";
                            return result;
                        }
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Update calendar failed";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update calendar failed";
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

        public async Task<ResultModel> GetServiceCalendarsByServiceOrder(string token, Guid serviceOrderID)
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
                List<TblServiceCalendar> resGet = await _serviceCalendarRepo.GetServiceCalendarsByServiceOrder(serviceOrderID);
                if (resGet.Any())
                {
                    List<ServiceCalendarResModel> resModel = new();
                    foreach (TblServiceCalendar calendar in resGet)
                    {
                        List<string> ImgUrls = await _imageRepo.GetImgUrlServiceCalendar(calendar.Id);
                        ServiceCalendarResModel serviceCalendarResModel = new()
                        {
                            Id = calendar.Id,
                            ServiceOrderId = calendar.ServiceOrderId,
                            ServiceDate = calendar.ServiceDate,
                            NextServiceDate = calendar.ServiceDate,
                            Sumary = calendar.Sumary,
                            Status = calendar.Status,
                            Images = ImgUrls,
                        };
                        resModel.Add(serviceCalendarResModel);
                    }
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resModel;
                    result.Message = "Get calendar success";
                    return result;
                }
                else
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resGet;
                    result.Message = "List empty";
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

        public async Task<ResultModel> GetServiceCalendarsByTechnician(string token, GetServiceCalendarsByTechnician getServiceCalendarsByTechnician)
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
                ServiceCalendarGetModel res = await _serviceCalendarRepo.GetServiceCalendarsByTechnician(getServiceCalendarsByTechnician.TechnicianID, getServiceCalendarsByTechnician.Date);
                if (res != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = res;
                    result.Message = "Get calendar success";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get calendar failed.";
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

        public async Task<ResultModel> GetServiceCalendarsByUser(string token, GetServiceCalendarsByUser getServiceCalendarsByUser)
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
                List<ServiceCalendarUserGetModel> resList = await _serviceCalendarRepo.GetServiceCalendarsByUser(getServiceCalendarsByUser.UserID, getServiceCalendarsByUser.StartDate, getServiceCalendarsByUser.EndDate);
                if (resList != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resList;
                    result.Message = "Get calendar success";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get calendar failed.";
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

        public async Task<ResultModel> GetServiceCalendarsTodayByTechnician(string token)
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

                /*ServiceCalendarGetModel res = await _serviceCalendarRepo.GetServiceCalendarsByTechnician(getServiceCalendarsByTechnician.TechnicianID, getServiceCalendarsByTechnician.Date);
                if (res != null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = res;
                    result.Message = "Get calendar success";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get calendar failed.";
                    return result;
                }*/
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
            return result;
        }
    }
}



