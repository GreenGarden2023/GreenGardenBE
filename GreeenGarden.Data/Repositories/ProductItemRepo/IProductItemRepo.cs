using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public interface IProductItemRepo : IRepository<TblProductItem>
    {
        Task<Page<TblProductItem>> GetProductItems(PaginationRequestModel pagingModel, Guid productID, Guid? SizeID, string? type, string? status);
        Task<bool> UpdateProductItem(ProductItemUpdateModel productItemUpdateModel);
    }
}
