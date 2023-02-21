using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.CategogyService
{
    public interface ICategogyService
    {
        Task<ResultModel> getAllCategories(PaginationRequestModel pagingModel);
        Task<ResultModel> GetCategoryByStatus(PaginationRequestModel pagingModel, string status);
        Task<ResultModel> createCategory(string token, CategoryCreateModel categoryCreateModel);
        Task<ResultModel> updateCategory(string token, CategoryUpdateModel categoryUpdateModel);
    }
}
