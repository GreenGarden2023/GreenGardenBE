using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
    public interface ISizeProductItemRepo : IRepository<TblProductItemDetail>
    {
        Task<List<ProductItemDetailResModel>> GetSizeProductItems(Guid productItemId, string? status);
        Task<bool> UpdateSizeProductItem(ProductItemDetailModel productItemDetailModel);
    }
}

