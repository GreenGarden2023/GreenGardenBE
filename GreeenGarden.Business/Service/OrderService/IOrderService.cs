using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> createOrder(string token, OrderModel model);
        Task<ResultModel> getDetailAddendum(Guid addendumId);
        Task<ResultModel> getListAddendum(string token, Guid orderId);
        Task<ResultModel> payByCashForAddendum(string token, Guid addendumId);

    }
}
