using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceOrderService
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly IServiceOrderRepo _serviceRepo;

        public ServiceOrderService(IServiceOrderRepo serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task<ResultModel> createServiceOrder(string token, ServiceOrderCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                double totalPrice = 0;
                double deposit = 0;
                DateTime serviceEndDate = ConvertUtil.convertStringToDateTime(model.ServiceEndDate);
                DateTime serviceStartDate = ConvertUtil.convertStringToDateTime(model.ServiceStartDate);
                double rangeDate = (serviceEndDate - serviceStartDate).TotalDays;

                // Tính toán các khoản:
                foreach (var item in model.RequestDetailModel)
                {
                    totalPrice += item.Price * rangeDate;
                }
                deposit= totalPrice / 2;

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
                    UserId = model.TechnicianID
                };
                await _serviceRepo.Insert(newServiceOrder);


                // Chỉnh sửa, remove requestDetail:
                foreach (var item in model.RequestDetailModel)
                {
                    var 
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
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
