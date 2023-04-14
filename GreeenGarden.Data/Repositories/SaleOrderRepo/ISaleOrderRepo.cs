using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SaleOrderRepo
{
    public interface ISaleOrderRepo : IRepository<TblSaleOrder>
    {
        Task<Page<TblSaleOrder>> GetAllSaleOrders(PaginationRequestModel paginationRequestModel);
        Task<Page<TblSaleOrder>> GetSaleOrders(PaginationRequestModel paginationRequestModel, Guid userID);
        Task<Page<TblSaleOrder>> GetSaleOrderByOrderCode(OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<ResultModel> UpdateSaleOrderStatus(Guid saleOrderID, string status);
        Task<ResultModel> UpdateSaleOrderDeposit(Guid saleOrderID);
        Task<ResultModel> UpdateSaleOrderRemain(Guid saleOrderID, double amount);
        Task<bool> UpdateSaleOrder(TblSaleOrder entity);
        Task<bool> CheckOrderCode(string code);
    }
}

