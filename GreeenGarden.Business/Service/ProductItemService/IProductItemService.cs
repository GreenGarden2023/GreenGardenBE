using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {
        Task<ResultModel> getAllProductItemByProductItemSize(PaginationRequestModel pagingModel, Guid productSizeId);
        Task<ResultModel> getSizesOfProduct(PaginationRequestModel pagingModel, Guid productId);
        Task<ResultModel> getDetailItem(Guid productItemId);
        Task<ResultModel> createProductItem(ProductItemCreateRequestModel model, IList<IFormFile> imgFile, string token);
    }
}
