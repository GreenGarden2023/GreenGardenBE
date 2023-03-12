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

        Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderID);

        Task<ResultModel> CancelRentOrder(string token, Guid rentOrderID);

        Task<ResultModel> GetSaleOrders(string token, PaginationRequestModel pagingModel);

        Task<ResultModel> GetSaleOrderDetail(string token, Guid saleOrderID);

        Task<ResultModel> CancelSaleOrder(string token, Guid saleOrderID);
    }
}

