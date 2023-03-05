using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public interface IProductItemRepo : IRepository<TblProductItem>
    {
        Task<Page<TblProductItem>> GetProductItemByType(PaginationRequestModel paginationRequestModel, string? type);
        Task<bool> UpdateProductItem(ProductItemUpdateModel productItemModel);
    }
}
