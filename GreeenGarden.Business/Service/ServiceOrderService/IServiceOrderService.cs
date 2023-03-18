using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceOrderService
{
    public interface IServiceOrderService
    {
        Task<ResultModel> createServiceOrder(string token, ServiceOrderCreateModel model);
        Task<ResultModel> updateServiceOrder(string token, ServiceUserUpdateModel model);
        Task<ResultModel> getListServiceOrderByCustomer(string token);
        Task<ResultModel> getListServiceOrderByManager(string token);
        Task<ResultModel> changeStatus(string token, ServiceOrderChangeStatusModel model);
        Task<ResultModel> getDetailServiceOrder(string token, Guid SerOrderID);
        Task<ResultModel> getTechnician(string token);
        Task<ResultModel> cleanServiceOrder(string token);
        Task<ResultModel> getListServiceOrderByTechnician(string token);
    }
}
