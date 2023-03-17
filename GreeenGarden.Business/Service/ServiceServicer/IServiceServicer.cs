using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceServicer
{
    public interface IServiceServicer
    {
        Task<ResultModel> createService(string token, ServiceCreateModel model);
        Task<ResultModel> getListServiceByCustomer(string token);
        Task<ResultModel> getListServiceByManager(string token);
        Task<ResultModel> changeStatus(string token, Guid serviceID, string status);
        Task<ResultModel> getDetailServiceByCustomer(string token, Guid serviceID);
        Task<ResultModel> updateService(string token, ServiceUpdateModel model);
    }
}
