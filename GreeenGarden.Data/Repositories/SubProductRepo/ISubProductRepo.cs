using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SubProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SubProductRepo
{
    public interface ISubProductRepo : IRepository<TblSubProduct>
    {
        public Task<TblSubProduct> queryDetailBySubId(Guid? SubId);

        public bool checkSizeUnique(Guid? SubId);
        public void updateSubProduct(TblSubProduct subProduct);
        public Task<SubProductAndSize> querySubAndSize(Guid SubId);
        public void updateWhenCreateItemUnique(Guid subId, double price);
        public void updateWhenUpdateItemSimilar(Guid guid);
    }
}
