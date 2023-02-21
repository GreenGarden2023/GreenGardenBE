using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> checkWholesaleProduct(Guid subProductId, int quantity);
        Task<ResultModel> checkRetailProduct(Guid productItemId);
        Task<ResultModel> createOrder(string token, Guid productItem, Guid productSize, int quantity, DateTime startDate, DateTime endDate);
    }
}
