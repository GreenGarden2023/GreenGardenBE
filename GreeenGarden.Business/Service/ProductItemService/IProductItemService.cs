using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeProductItemModel;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {
        Task<ResultModel> CreateProductItem(string token, ProductItemInsertModel productItemInsertModel);
        Task<ResultModel> CreateProductItemDetail(string token, ProductItemDetailModel productItemDetailModel);
        Task<ResultModel> GetProductItem(PaginationRequestModel pagingModel, Guid productID, string? status, string? type);
        Task<ResultModel> GetDetailProductItem(Guid productItemID, string? sizeProductItemStatus);
        Task<ResultModel> UpdateProductItem(string token, ProductItemUpdateModel productItemModel);
        Task<ResultModel> UpdateProductItemDetail(string token, ProductItemDetailModel productItemDetailModel);
    }
}
