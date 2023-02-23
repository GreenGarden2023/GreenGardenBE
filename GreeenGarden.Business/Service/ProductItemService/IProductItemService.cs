using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {

        Task<ResultModel> GetProductItems(PaginationRequestModel pagingModel, Guid productID, Guid? sizeID, string? type, string? status);
        Task<ResultModel> CreateProductItems(ProductItemCreateModel productItemCreateModel);
    }
}
