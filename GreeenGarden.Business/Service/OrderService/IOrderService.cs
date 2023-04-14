using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> CreateRentOrder(string token, OrderCreateModel rentOrderModel);
        Task<ResultModel> CalculateRentOrder(OrderCreateModel rentOrderModel);

        Task<ResultModel> CreateSaleOrder(string token, OrderCreateModel saleOrderModel);
        Task<ResultModel> CalculateSaleOrder(OrderCreateModel saleOrderModel);

        Task<ResultModel> GetRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetRentOrdersByGroup(string token, Guid groupID);

        Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderID);
        Task<ResultModel> GetRentOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> SearchRentOrderDetail(string token, OrderFilterModel model, PaginationRequestModel pagingModel);

        Task<ResultModel> UpdateRentOrderStatus(string token, Guid rentOrderID, string status);
        Task<ResultModel> CancelRentOrderById(string token, Guid rentOrderID, string reason);

        Task<ResultModel> GetSaleOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllSaleOrders(string token, PaginationRequestModel pagingModel);

        Task<ResultModel> GetSaleOrderDetail(string token, Guid saleOrderID);
        Task<ResultModel> GetSaleOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel);

        Task<ResultModel> GetARentOrder(string token, Guid rentOrderID);

        Task<ResultModel> UpdateSaleOrderStatus(string token, Guid saleOrderID, string status);
        Task<ResultModel> CancelSaleOrderById(string token, Guid saleOrderID, string reason);

        Task<ResultModel> CompleteServiceOrderStatus(string token, Guid serviceOrderID);
        Task<ResultModel> CreateServiceOrder(string token, ServiceOrderCreateModel serviceOrderCreateModel);
        Task<ResultModel> GetServiceOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllServiceOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetServiceOrderByTechnician(string token, PaginationRequestModel pagingModel, Guid technicianID);
        Task<ResultModel> GetServiceOrderById(string token, Guid orderID);
        Task<ResultModel> GetServiceOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> CancelServiceOrderById(string token, Guid orderID, string reason);
    }
}

