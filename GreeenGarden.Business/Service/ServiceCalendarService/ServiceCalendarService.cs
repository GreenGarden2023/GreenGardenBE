using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Entities;

namespace GreeenGarden.Business.Service.ServiceCalendarService
{
	public class ServiceCalendarService : IServiceCalendarService
	{
        private readonly DecodeToken _decodeToken;
        private readonly IServiceCalendarRepo _serviceCalendarRepo;
        public ServiceCalendarService(IServiceCalendarRepo serviceCalendarRepo)
		{
            _decodeToken = new DecodeToken();
            _serviceCalendarRepo = serviceCalendarRepo;
        }

        public async Task<ResultModel> CreateServiceCalendar(string token, ServiceCalendarInsertModel serviceCalendarInsertModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN))
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
                if(serviceCalendarInsertModel.CalendarInitial != null && serviceCalendarInsertModel.CalendarUpdate == null)
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
                        ServiceCalendarResModel resModel = new ServiceCalendarResModel
                        {
                            Id = resGetModel.Id,
                            ServiceOrderId = resGetModel.ServiceOrderId,
                            ServiceDate = resGetModel.ServiceDate,
                            NextServiceDate = resGetModel.NextServiceDate,
                            ReportFileURL = resGetModel.ReportFileUrl,
                            Sumary = resGetModel.Sumary,
                            Status = resGetModel.Status,
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
                        ServiceCalendarResModel oldCalendarRes = new ServiceCalendarResModel
                        {
                            Id = oldCalendarGet.Id,
                            ServiceOrderId = oldCalendarGet.ServiceOrderId,
                            ServiceDate = oldCalendarGet.ServiceDate,
                            NextServiceDate = oldCalendarGet.NextServiceDate,
                            ReportFileURL = oldCalendarGet.ReportFileUrl,
                            Sumary = oldCalendarGet.Sumary,
                            Status = oldCalendarGet.Status,
                        };
                        TblServiceCalendar tblServiceCalendar = new TblServiceCalendar
                        {
                            Id = Guid.NewGuid(),
                            ServiceOrderId = oldCalendarRes.ServiceOrderId,
                            ServiceDate = serviceCalendarInsertModel.CalendarUpdate.NextServiceDate,
                            Status = ServiceCalendarStatus.PENDING
                        };
                        Guid insert = await _serviceCalendarRepo.Insert(tblServiceCalendar);
                        if(insert != Guid.Empty)
                        {
                           
                            TblServiceCalendar nextCalendarGet = await _serviceCalendarRepo.Get(tblServiceCalendar.Id);
                            ServiceCalendarResModel nextCalendarRes = new ServiceCalendarResModel
                            {
                                Id = nextCalendarGet.Id,
                                ServiceOrderId = nextCalendarGet.ServiceOrderId,
                                ServiceDate = nextCalendarGet.ServiceDate,
                                NextServiceDate = nextCalendarGet.NextServiceDate,
                                ReportFileURL = nextCalendarGet.ReportFileUrl,
                                Sumary = nextCalendarGet.Sumary,
                                Status = nextCalendarGet.Status,
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
    }
}

