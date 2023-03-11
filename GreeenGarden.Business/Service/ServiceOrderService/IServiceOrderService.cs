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
        Task<ResultModel> showTechnician(string token);
        Task<ResultModel> showListServiceOrder(string token);
    }
}
