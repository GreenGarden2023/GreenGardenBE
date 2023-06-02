using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;

using GreeenGarden.Data.Models.TakecareComboCalendarModel;
using GreeenGarden.Data.Repositories.ComboServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Repositories.TakecareComboOrderRepo;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Utilities.Convert;

namespace GreeenGarden.Business.Service.TakecareComboCalendarService
{
	public class TakecareComboCalendarService : ITakecareComboCalendarService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IComboServiceCalendarRepo _comboServiceCalendarRepo;
        private readonly IImageRepo _imageRepo;
        private readonly IUserRepo _userRepo;
        private readonly ITakecareComboOrderRepo _takecareComboOrderRepo;
        private readonly IEMailService _emailService;

        public TakecareComboCalendarService(IComboServiceCalendarRepo comboServiceCalendarRepo, IImageRepo imageRepo, IUserRepo userRepo, ITakecareComboOrderRepo takecareComboOrderRepo, IEMailService eMailService)
		{
            _decodeToken = new DecodeToken();
            _comboServiceCalendarRepo = comboServiceCalendarRepo;
            _imageRepo = imageRepo;
            _userRepo = userRepo;
            _takecareComboOrderRepo = takecareComboOrderRepo;
            _emailService = eMailService;
        }

        public async Task<ResultModel> CreateServiceCalendar(string token, TakecareComboCalendarInsertModel serviceCalendarInsertModel)
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
                    TblComboServiceCalendar tblServiceCalendar = new()
                    {
                        Id = Guid.NewGuid(),
                        TakecareComboOrderId = serviceCalendarInsertModel.CalendarInitial.ServiceOrderId,
                        ServiceDate = DateTime.ParseExact(serviceCalendarInsertModel.CalendarInitial.ServiceDate, "dd/MM/yyyy", null),
                        Status = ServiceCalendarStatus.PENDING
                    };
                    Guid insert = await _comboServiceCalendarRepo.Insert(tblServiceCalendar);
                    if (insert != Guid.Empty)
                    {
                        TblComboServiceCalendar resGetModel = await _comboServiceCalendarRepo.Get(tblServiceCalendar.Id);
                        List<string> ImgUrls = await _imageRepo.GetImgUrlServiceCalendar(resGetModel.Id);
                        ComboServiceCalendarResModel resModel = new()
                        {
                            Id = resGetModel.Id,
                            ServiceOrderId = resGetModel.TakecareComboOrderId,
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
                    bool update = await _comboServiceCalendarRepo.UpdateServiceCalendar(serviceCalendarInsertModel.CalendarUpdate);
                    if (update == true)
                    {
                        TblComboServiceCalendar oldCalendarGet = await _comboServiceCalendarRepo.Get(serviceCalendarInsertModel.CalendarUpdate.ServiceCalendarId);
                        bool updateImg = await _imageRepo.UpdateImgForComboServiceCalendar(oldCalendarGet.Id, serviceCalendarInsertModel.CalendarUpdate.Images);
                        List<string> ImgUrls = await _imageRepo.GetImgUrlComboServiceCalendar(oldCalendarGet.Id);
                        ComboServiceCalendarResModel oldCalendarRes = new()
                        {
                            Id = oldCalendarGet.Id,
                            ServiceOrderId = oldCalendarGet.TakecareComboOrderId,
                            ServiceDate = oldCalendarGet.ServiceDate,
                            NextServiceDate = oldCalendarGet.NextServiceDate,
                            Sumary = oldCalendarGet.Sumary,
                            Status = oldCalendarGet.Status,
                            Images = ImgUrls
                        };

                        if (serviceCalendarInsertModel.CalendarUpdate.NextServiceDate != null)
                        {
                            TblComboServiceCalendar tblServiceCalendar = new()
                            {
                                Id = Guid.NewGuid(),
                                TakecareComboOrderId = oldCalendarRes.ServiceOrderId,
                                ServiceDate = DateTime.ParseExact(serviceCalendarInsertModel.CalendarUpdate.NextServiceDate, "dd/MM/yyyy", null),
                                Status = ServiceCalendarStatus.PENDING
                            };
                            Guid insert = await _comboServiceCalendarRepo.Insert(tblServiceCalendar);
                            if (insert != Guid.Empty)
                            {

                                TblComboServiceCalendar nextCalendarGet = await _comboServiceCalendarRepo.Get(tblServiceCalendar.Id);
                                List<string> nextCaImgUrls = await _imageRepo.GetImgUrlComboServiceCalendar(nextCalendarGet.Id);
                                ComboServiceCalendarResModel nextCalendarRes = new()
                                {
                                    Id = nextCalendarGet.Id,
                                    ServiceOrderId = nextCalendarGet.TakecareComboOrderId,
                                    ServiceDate = nextCalendarGet.ServiceDate,
                                    NextServiceDate = nextCalendarGet.NextServiceDate,
                                    Sumary = nextCalendarGet.Sumary,
                                    Status = nextCalendarGet.Status,
                                    Images = nextCaImgUrls
                                };
                                ComboServiceCalendarUpdateResModel serviceCalendarUpdateResModel = new()
                                {
                                    PreviousCalendar = oldCalendarRes,
                                    NextCalendar = nextCalendarRes
                                };
                                TblTakecareComboOrder tblServiceOrder = await _takecareComboOrderRepo.Get(oldCalendarRes.ServiceOrderId);
                                TblUser tblUser = await _userRepo.Get((Guid)tblServiceOrder.UserId);
                                _ = await _emailService.SendEmailComboReportUpdate(tblUser.Mail, oldCalendarRes.Id);
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
                            TblTakecareComboOrder tblServiceOrder = await _takecareComboOrderRepo.Get(oldCalendarRes.ServiceOrderId);
                            //bool updateService = await _serviceRepo.ChangeServiceStatus(tblServiceOrder.ServiceId, ServiceStatus.COMPLETED);
                            ComboServiceCalendarUpdateResModel serviceCalendarUpdateResModel = new()
                            {
                                PreviousCalendar = oldCalendarRes,
                                NextCalendar = null
                            };
                            TblUser tblUser = await _userRepo.Get((Guid)tblServiceOrder.UserId);
                            _ = await _emailService.SendEmailComboReportUpdate(tblUser.Mail, oldCalendarRes.Id);
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
                List<TblComboServiceCalendar> resGet = await _comboServiceCalendarRepo.GetServiceCalendarsByServiceOrder(serviceOrderID);
                if (resGet.Any())
                {
                    List<ComboServiceCalendarResModel> resModel = new();
                    foreach (TblComboServiceCalendar calendar in resGet)
                    {
                        List<string> ImgUrls = await _imageRepo.GetImgUrlComboServiceCalendar(calendar.Id);
                        ComboServiceCalendarResModel serviceCalendarResModel = new()
                        {
                            Id = calendar.Id,
                            ServiceOrderId = calendar.TakecareComboOrderId,
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

        public async Task<ResultModel> GetServiceCalendarsByTechnician(string token, GetComboServiceCalendarsByTechnician getServiceCalendarsByTechnician)
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
                ComboServiceCalendarGetModel res = await _comboServiceCalendarRepo.GetServiceCalendarsByTechnician(getServiceCalendarsByTechnician.TechnicianID, getServiceCalendarsByTechnician.Date);
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

        public async Task<ResultModel> GetServiceCalendarsByUser(string token, GetComboServiceCalendarsByUser getServiceCalendarsByUser)
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
                List<ComboServiceCalendarUserGetModel> resList = await _comboServiceCalendarRepo.GetServiceCalendarsByUser(getServiceCalendarsByUser.UserID, getServiceCalendarsByUser.StartDate, getServiceCalendarsByUser.EndDate);
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

