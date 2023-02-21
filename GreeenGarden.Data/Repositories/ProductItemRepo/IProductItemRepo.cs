using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public interface IProductItemRepo : IRepository<TblProductItem>
    {
        /*public Page<TblSubProduct> queryAllSizeByProduct(PaginationRequestModel model, Guid productId);
        public Page<TblProductItem> queryAllItemByProductSize(PaginationRequestModel model, Guid productSizeId);*/
        public IQueryable<TblProductItem> queryDetailItemByProductSize(Guid productItemId);
        public List<string> getImgByProductItem(Guid productItemId);
    }
}
