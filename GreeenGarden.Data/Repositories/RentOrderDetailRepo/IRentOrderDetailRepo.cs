using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RentOrderDetailRepo
{
    public interface IRentOrderDetailRepo : IRepository<TblRentOrderDetail>
    {
        Task<List<RentOrderDetailResModel>> GetRentOrderDetails(Guid RentOrderId);
        Task<List<TblRentOrderDetail>> GetRentOrderDetailsByRentOrderID(Guid RentOrderId);
        Task<bool> UpdateRentOrderDetail(TblRentOrderDetail entity);
    }
}

