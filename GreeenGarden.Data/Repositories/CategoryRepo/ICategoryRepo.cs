using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.CategoryRepo
{
    public interface ICategoryRepo : IRepository<TblCategory>
    {
        public Task<Page<TblCategory>> GetCategoryByStatus(PaginationRequestModel pagingModel, string status);
        public Task<Page<TblCategory>> GetAllCategory(PaginationRequestModel pagingModel);
        public string getImgByCategory(Guid categoryId);
        public Task<TblCategory> selectDetailCategory(Guid categoryId);
        public Task<TblCategory> updateCategory(CategoryUpdateModel categoryUpdateModel);
        public bool checkCategoryNameExist(string categoryName);
        public bool checkCategoryIDExist(Guid categoryID);
        public Task<bool> updateStatus(CategoryUpdateStatusModel model);
    }
}
