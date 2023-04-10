using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
    public interface IRentOrderRepo : IRepository<TblRentOrder>
    {
        Task<List<TblRentOrder>> GetRentOrders(Guid userID);
        Task<TblRentOrder> GetRentOrderByOrderCode(string orderCode);
        Task<Page<TblRentOrder>> GetRentOrderByDate(DateTime fromDate, DateTime toDate, PaginationRequestModel pagingModel);
        Task<List<TblRentOrder>> GetRentOrdersByGroup(Guid rentOrderGroupID);
        Task<ResultModel> UpdateRentOrderStatus(Guid rentOrderID, string status);
        Task<ResultModel> UpdateRentOrderDeposit(Guid rentOrderID);
        Task<ResultModel> UpdateRentOrderRemain(Guid rentOrderID, double amount);
        Task<bool> CheckOrderCode(string Code);
    }
}

