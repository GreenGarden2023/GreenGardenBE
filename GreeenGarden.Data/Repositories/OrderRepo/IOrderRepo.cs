using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.AddendumModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public interface IOrderRepo : IRepository<TblOrder>
    {
        public Task<TblProductItem> getProductToCompare(Guid productId);
        public Task<TblUser> getUserByUsername(string username);
        public Task<TblAddendum> insertAddendum(TblAddendum entities);
        public Task<TblAddendumProductItem> insertAddendumProductItem(TblAddendumProductItem entities);
        public Task<TblProductItem> minusQuantityProductItem(Guid productItemId, int quantity);
        public Task<List<listAddendumResponse>> getListAddendum(Guid OrderId);
        public Task<AdddendumResponseModel> getDetailAddendum(Guid AddendumId);
        public Task<TblAddendum> GetAddendum(Guid AddendumId);
        public Task<TblOrder> GetOrder(Guid OrderId);

    }
}
