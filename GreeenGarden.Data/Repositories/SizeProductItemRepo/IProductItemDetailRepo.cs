using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
    public interface IProductItemDetailRepo : IRepository<TblProductItemDetail>
    {
        Task<List<ProductItemDetailResModel>> GetSizeProductItems(Guid productItemId, string? status);
        Task<bool> UpdateSizeProductItem(ProductItemDetailModel productItemDetailModel);
        Task<bool> UpdateProductItemDetailQuantity(Guid productItemDetailID, int deductQuantity);
        Task<bool> UpdateProductItemDetail(TblProductItemDetail entity);
    }
}

