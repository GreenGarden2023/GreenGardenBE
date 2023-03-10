using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
	public interface IOrderService
	{
		Task<ResultModel> CreateRentOrder(string token, OrderCreateModel  rentOrderModel);

        Task<ResultModel> CreateSaleOrder(string token, OrderCreateModel saleOrderModel);

        Task<ResultModel> GetRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllRentOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetRentOrdersByGroup(string token, Guid groupID);

        Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderID);

        Task<ResultModel> UpdateRentOrderStatus(string token, Guid rentOrderID, string status);

        Task<ResultModel> GetSaleOrders(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> GetAllSaleOrders(string token, PaginationRequestModel pagingModel);

        Task<ResultModel> GetSaleOrderDetail(string token, Guid saleOrderID);

        Task<ResultModel> UpdateSaleOrderStatus(string token, Guid saleOrderID, string status);
    }
}

