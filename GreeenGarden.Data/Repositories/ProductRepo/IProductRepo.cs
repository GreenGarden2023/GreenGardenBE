using System.ComponentModel.DataAnnotations;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductRepo
{
    public interface IProductRepo : IRepository<TblProduct>
    {
        public Task<Page<TblProduct>> queryAllProduct(PaginationRequestModel pagingModel);
        public Task<Page<TblProduct>> queryAllProductByCategoryAndStatus(PaginationRequestModel pagingModel,Guid categoryID, string? status, string? rentSale);
        public Task<TblProduct> queryAProductByProId(Guid? proId);
        public Task<bool> UpdateProduct(ProductUpdateModel productUpdateModel);
        public void increaseQuantity(Guid subId, int plus);
    }
}
