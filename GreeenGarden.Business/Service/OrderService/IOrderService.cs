using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> CreateRentOrder(string token, OrderCreateModel rentOrderModel);
        Task<ResultModel> CalculateRentOrder(OrderCreateModel rentOrderModel);
        Task<ResultModel> CreateSaleOrder(string token, OrderCreateModel saleOrderModel);
        Task<ResultModel> CalculateSaleOrder(OrderCreateModel saleOrderModel);
        Task<ResultModel> GetRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetRentOrderGroupByOrderID(string token, Guid orderID);
        Task<ResultModel> GetAllRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetRentOrdersByGroup(string token, Guid groupID);
        Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderID);
        Task<ResultModel> GetRentOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> GetServiceOrderDetailByRangeDate(string token, OrderRangeDateReqModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> SearchRentOrderDetail(string token, OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> UpdateRentOrderStatus(string token, Guid rentOrderID, string status);
        Task<ResultModel> CancelRentOrderById(string token, Guid rentOrderID, string reason);
        Task<ResultModel> GetSaleOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllSaleOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetSaleOrderDetail(string token, Guid saleOrderID);
        Task<ResultModel> GetSaleOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> GetARentOrder(string token, Guid rentOrderID);
        Task<ResultModel> UpdateSaleOrderStatus(string token, Guid saleOrderID, string status);
        Task<ResultModel> UpdateServiceOrderStatus(string token, UpdateServiceOrderStatusModel model);
        Task<ResultModel> CancelSaleOrderById(string token, Guid saleOrderID, string reason);
        Task<ResultModel> CompleteServiceOrderStatus(string token, Guid serviceOrderID);
        Task<ResultModel> CreateServiceOrder(string token, ServiceOrderCreateModel serviceOrderCreateModel);
        Task<ResultModel> GetServiceOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllServiceOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetServiceOrderByTechnician(string token, PaginationRequestModel pagingModel, Guid technicianID);
        Task<ResultModel> GetServiceOrderByTechnicianToday(string token, PaginationRequestModel pagingModel, Guid technicianID, string status, bool nextDay);
        Task<ResultModel> GetServiceOrderById(string token, Guid orderID);
        Task<ResultModel> GetServiceOrderDetailByOrderCode(string token, OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> CancelServiceOrderById(string token, Guid orderID, string reason);
        Task<ResultModel> UpdateDateTakecare(string token, UpdateDateTakecareModel model);
        Task<ResultModel> UpdateCareGuideByTechnician(string token, UpdateCareGuideByTechnModel model);
        Task<ResultModel> GeneratePDF(Guid orderCode);
        Task<ResultModel> GenerateCareGuidePDF(Guid orderCode, int flag);
        Task<ResultModel> GenerateServiceCareGuidePDF(Guid orderCode);
        Task<ResultModel> GenerateCareGuidePDFForService(List<ServiceDetailResModel> listItem);

    }
}

