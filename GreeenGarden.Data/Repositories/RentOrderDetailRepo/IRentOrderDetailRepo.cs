using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RentOrderDetailRepo
{
	public interface IRentOrderDetailRepo : IRepository<TblRentOrderDetail>
	{
		Task<List<RentOrderDetailResModel>> GetRentOrderDetails(Guid RentOrderId);
	}
}

