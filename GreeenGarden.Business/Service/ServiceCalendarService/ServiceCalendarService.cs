using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Entities;
using Newtonsoft.Json.Linq;
using GreeenGarden.Data.Repositories.ImageRepo;

namespace GreeenGarden.Business.Service.ServiceCalendarService
{
    public class ServiceCalendarService : IServiceCalendarService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IServiceCalendarRepo _serviceCalendarRepo;
        private readonly IImageRepo _imageRepo;
        public ServiceCalendarService(IServiceCalendarRepo serviceCalendarRepo, IImageRepo imageRepo)
        {
            _decodeToken = new DecodeToken();
            _serviceCalendarRepo = serviceCalendarRepo;
            _imageRepo = imageRepo;
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
                    TblServiceCalendar tblServiceCalendar = new TblServiceCalendar
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
                        ServiceCalendarResModel resModel = new ServiceCalendarResModel
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
                        ServiceCalendarResModel oldCalendarRes = new ServiceCalendarResModel
                        {
                            Id = oldCalendarGet.Id,
                            ServiceOrderId = oldCalendarGet.ServiceOrderId,
                            ServiceDate = oldCalendarGet.ServiceDate,
                            NextServiceDate = oldCalendarGet.NextServiceDate,
                            Sumary = oldCalendarGet.Sumary,
                            Status = oldCalendarGet.Status,
                            Images = ImgUrls
                        };
                        TblServiceCalendar tblServiceCalendar = new TblServiceCalendar
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
                            ServiceCalendarResModel nextCalendarRes = new ServiceCalendarResModel
                            {
                                Id = nextCalendarGet.Id,
                                ServiceOrderId = nextCalendarGet.ServiceOrderId,
                                ServiceDate = nextCalendarGet.ServiceDate,
                                NextServiceDate = nextCalendarGet.NextServiceDate,
                                Sumary = nextCalendarGet.Sumary,
                                Status = nextCalendarGet.Status,
                                Images = nextCaImgUrls
                            };
                            ServiceCalendarUpdateResModel serviceCalendarUpdateResModel = new ServiceCalendarUpdateResModel
                            {
                                PreviousCalendar = oldCalendarRes,
                                NextCalendar = nextCalendarRes
                            };
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
                    List<ServiceCalendarResModel> resModel = new List<ServiceCalendarResModel>();
                    foreach (TblServiceCalendar calendar in resGet)
                    {
                        List<string> ImgUrls = await _imageRepo.GetImgUrlServiceCalendar(calendar.Id);
                        ServiceCalendarResModel serviceCalendarResModel = new ServiceCalendarResModel
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
        }
        }
   
        

