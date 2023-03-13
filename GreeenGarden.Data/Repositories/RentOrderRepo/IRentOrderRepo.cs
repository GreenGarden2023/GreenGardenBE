using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
	public interface IRentOrderRepo : IRepository<TblRentOrder>
	{
		Task<List<TblRentOrder>> GetRentOrders(Guid userID);
        Task<List<TblRentOrder>> GetRentOrdersByGroup(Guid rentOrderGroupID);
        Task<ResultModel> UpdateRentOrderStatus(Guid rentOrderID, string status);
        Task<ResultModel> UpdateRentOrderDeposit(Guid rentOrderID);
        Task<ResultModel> UpdateRentOrderRemain(Guid rentOrderID, double amount);
        Task<bool> CheckOrderCode(string Code);
    }
}

