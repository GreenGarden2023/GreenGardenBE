using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceOrderService
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly IServiceOrderRepo _serviceRepo;
        private readonly DecodeToken _decodeToken;

        public ServiceOrderService(IServiceOrderRepo serviceRepo)
        {
            _serviceRepo = serviceRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> createServiceOrder(string token, ServiceOrderCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }


                double? totalPrice = 0;
                double? deposit = 0;
                DateTime serviceEndDate = ConvertUtil.convertStringToDateTime(model.ServiceEndDate);
                DateTime serviceStartDate = ConvertUtil.convertStringToDateTime(model.ServiceStartDate);
                double rangeDate = (serviceEndDate - serviceStartDate).TotalDays;

                // Tính toán các khoản:
                foreach (var item in model.RequestDetailModel)
                {
                    totalPrice += item.Price * rangeDate;
                }
                deposit = totalPrice / 2;

                //CreateServiceOrder
                var newServiceOrder = new TblServiceOrder()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    ServiceStartDate = serviceStartDate,
                    ServiceEndDate = serviceEndDate,
                    Deposit = deposit,
                    TotalPrice = totalPrice,
                    Status = Status.UNPAID,
                    RewardPointGain = 0,
                    RewardPointUsed = 0,
                    RequestId = model.RequestID,
                    TechnicianId = model.TechnicianID
                };
                await _serviceRepo.Insert(newServiceOrder);


                // Chỉnh sửa, remove requestDetail:
                var curRequestDetail = await _serviceRepo.getListRequestDetail(model.RequestID);
                var removeRequestDetail = await _serviceRepo.getListRequestDetail(model.RequestID);

                foreach (var i in model.RequestDetailModel)
                {
                    foreach (var j in curRequestDetail)
                    {
                        if (i.ID == j.Id)
                        {
                            removeRequestDetail.Remove(j);
                        }
                    }
                    var newRequestDetail = new TblRequestDetail()
                    {
                        Id = i.ID,
                        TreeName = i.TreeName,
                        Quantity = i.Quantity,
                        Description = i.Description,
                        RequestId = model.RequestID,
                        ServiceOrderId = newServiceOrder.Id,
                        Price = i.Price,
                    };
                    await _serviceRepo.updateRequestDetail(newRequestDetail);
                }
                foreach (var r in removeRequestDetail)
                {
                    await _serviceRepo.removeRequestDetail(r);
                }
                

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Create service order successfully!";
                result.Data = newServiceOrder;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> showListServiceOrder(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serviceRepo.GetUser(_decodeToken.Decode(token, "username"));
                var response = await _serviceRepo.getServiceOrder(tblUser.Id);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = response;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> showTechnician(string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }

                var responseresponse = await _serviceRepo.getTechnician();



                result.Code = 200;
                result.IsSuccess = true;
                result.Data = responseresponse;
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
