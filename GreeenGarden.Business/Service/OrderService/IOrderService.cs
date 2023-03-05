using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> createOrder(string token, addToOrderModel model);
        Task<ResultModel> getDetailAddendum(Guid addendumId);
        Task<ResultModel> getListAddendum(string token, Guid orderId);
        Task<ResultModel> addAddendumByOrder(string token, addendumToAddByOrderModel model);
        Task<ResultModel> getListOrderByCustomer(string token); 
        Task<ResultModel> completeAddendum(string token, Guid addendumID);

        /******************Manager************/
        Task<ResultModel> getListOrderByManager(string token);
    }
}
