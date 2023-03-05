using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.AddendumModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public interface IOrderRepo : IRepository<TblOrder>
    {
        public Task<TblAddendum> insertAddendum(TblAddendum entities);
        public Task<TblAddendumProductItem> insertAddendumProductItem(TblAddendumProductItem entities);
        public Task<List<listAddendumResponse>> getListAddendum(Guid OrderId);
        public Task<AdddendumResponseModel> getDetailAddendum(Guid AddendumId);
        //public Task<TblAddendum> GetAddendum(Guid AddendumId);
        public Task<TblOrder> GetOrder(Guid OrderId);
        public Task<TblUser> GetUser(string username);
        public Task<TblRole> GetRole(Guid roleID);
        public Task<TblSizeProductItem> GetSizeProductItem(Guid sizeProductItemID);
        public Task<bool> minusQuantitySizeProductItem(Guid sizeProductItemID, int quantity);
        public Task<List<orderShowModel>> GetListOrder(Guid userID);
        public Task<bool> UpdateOrderPayment(Guid orderID);
        public Task<bool> updateStatusAddendum(Guid addendumID, string status);
        public Task<bool> updateStatusOrder(Guid orderID, string status);

        //*********************Manager********************//
        public Task<List<managerOrderModel>> getListOrderByManager();


    }
}
