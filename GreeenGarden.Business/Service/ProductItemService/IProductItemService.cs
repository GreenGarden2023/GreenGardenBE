using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {
        Task<ResultModel> CreateProductItem(string token, ProductItemInsertModel productItemInsertModel);
        Task<ResultModel> GetProductItem(PaginationRequestModel pagingModel, Guid productID, string? status, string? type);
    }
}
