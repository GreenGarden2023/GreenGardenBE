using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductRepo
{
    public interface IProductRepo : IRepository<TblProduct>
    {
        public Page<TblProduct> queryAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId);
        public Page<TblProduct> queryAllProduct(PaginationRequestModel pagingModel);
        public string getImgByProduct(Guid productId);
        public TblProduct queryAProductByProId(Guid? proId);
        public Task<bool> UpdateProduct(ProductUpdateModel productUpdateModel);
        public void increaseQuantity(Guid subId, int plus);
    }
}
