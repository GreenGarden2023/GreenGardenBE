using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {
        Task<ResultModel> CreateProductItem(string token, ProductItemInsertModel productItemInsertModel);
        Task<ResultModel> CreateProductItemSize(string token, SizeProductItemInsertModel sizeProductItemInsertModel);
        Task<ResultModel> GetProductItem(PaginationRequestModel pagingModel, Guid productID, string? status, string? type);
        Task<ResultModel> GetDetailProductItem(Guid productItemID, string sizeProductItemStatus);
        Task<ResultModel> UpdateProductItem(string token, ProductItemUpdateModel productItemModel);
        Task<ResultModel> UpdateSizeProductItem(string token, SizeProductItemUpdateModel sizeProductItemResModel);
    }
}
