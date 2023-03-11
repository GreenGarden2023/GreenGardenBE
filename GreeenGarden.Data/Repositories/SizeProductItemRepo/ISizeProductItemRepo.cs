using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
    public interface ISizeProductItemRepo : IRepository<TblProductItemDetail>
    {
        Task<List<ProductItemDetailResModel>> GetSizeProductItems(Guid productItemId, string? status);
        Task<bool> UpdateSizeProductItem(ProductItemDetailModel productItemDetailModel);
    }
}

