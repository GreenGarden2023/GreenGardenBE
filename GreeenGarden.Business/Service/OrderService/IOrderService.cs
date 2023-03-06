using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> createOrder(string token, addToOrderModel model);
        Task<ResultModel> getDetailAddendum(string token, Guid addendumId);
        Task<ResultModel> getDetailOrder(string token, Guid orderId);
        Task<ResultModel> addAddendumByOrder(string token, addendumToAddByOrderModel model);
        Task<ResultModel> getListOrderByCustomer(string token); 

        /******************Manager************/
        Task<ResultModel> getListOrderByManager(string token, PaginationRequestModel paginationRequestModel);
        Task<ResultModel> deleteListOrder(string token);
        Task<ResultModel> changeStatusOrder(string token, Guid orderID, string status);
    }
}
