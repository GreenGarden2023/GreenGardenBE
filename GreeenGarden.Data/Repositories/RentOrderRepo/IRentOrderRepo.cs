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
        Task<Page<TblRentOrder>> SearchRentOrder(OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<Page<TblRentOrder>> GetRentOrderByDate(DateTime fromDate, DateTime toDate, PaginationRequestModel pagingModel);
        Task<List<TblRentOrder>> GetRentOrdersByGroup(Guid rentOrderGroupID);
        Task<ResultModel> UpdateRentOrderStatus(Guid rentOrderID, string status, string username = null);
        Task<ResultModel> UpdateRentOrderDeposit(Guid rentOrderID);
        Task<ResultModel> UpdateRentOrderRemain(Guid rentOrderID, double amount);
        Task<bool> UpdateRentOrder(TblRentOrder entity);
        Task<bool> CheckOrderCode(string Code);
        Task<ResultModel> UpdateRentOrderContractUrl(Guid rentOrderID, string contracURL);
        Task<bool> UpdateRentOrderCareGuideUrl(Guid rentOrderID, string careGuideURL);
        Task<bool> checkCodeDup(string code, int flag);
    }
}

