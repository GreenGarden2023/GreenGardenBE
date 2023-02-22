using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.ProductService
{
    public interface IProductService
    {
        Task<ResultModel> createProduct(ProductCreateModel model, string token);
        Task<ResultModel> getAllProduct(PaginationRequestModel pagingModel);
        Task<ResultModel> UpdateProduct(ProductUpdateModel model, string token);
        Task<ResultModel> getAllProductByCategoryStatus(PaginationRequestModel pagingModel, Guid categoryID, string? status);
    }
}
