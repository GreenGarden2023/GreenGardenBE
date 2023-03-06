using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.AddendumModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public interface IOrderRepo : IRepository<TblOrder>
    {
        public Task<TblAddendum> insertAddendum(TblAddendum entities);
        public Task<TblAddendumProductItem> insertAddendumProductItem(TblAddendumProductItem entities);
        public Task<orderDetail> getDetailOrder(Guid OrderId, int flag, Guid? addendumID);
        public Task<AdddendumResponseModel> getDetailAddendum(Guid AddendumId);
        //public Task<TblAddendum> GetAddendum(Guid AddendumId);
        public Task<TblOrder> GetOrder(Guid? OrderId, Guid? AddendumID);
        public Task<TblUser> GetUser(string username);
        public Task<Page<TblUser>> GetListUser(PaginationRequestModel paginationRequestModel);
        public Task<TblRole> GetRole(Guid roleID);
        public Task<TblSizeProductItem> GetSizeProductItem(Guid sizeProductItemID);
        public Task<bool> minusQuantitySizeProductItem(Guid sizeProductItemID, int quantity);
        public Task<listOrder> GetListOrder(Guid userID);
        public Task<bool> UpdateOrderPayment(Guid orderID);
        public Task<bool> updateStatusAddendum(Guid addendumID, string status);
        public Task<bool> updateStatusOrder(Guid orderID, string status);
        public Task<bool> removeCart(Guid userID);
        public Task<TblAddendum> getLastAddendum(Guid orderId);

        //*********************Manager********************//
        public Task<bool> removeOrder(Guid userID);


    }
}
