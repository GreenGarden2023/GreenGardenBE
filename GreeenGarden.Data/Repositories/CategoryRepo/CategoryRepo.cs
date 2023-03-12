using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.CategoryRepo
{
    public class CategoryRepo : Repository<TblCategory>, ICategoryRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public CategoryRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }


        public async Task<string> getImgByCategory(Guid categoryId)
        {
            TblImage? result = await _context.TblImages.Where(x => x.CategoryId.Equals(categoryId)).FirstOrDefaultAsync();
            return result?.ImageUrl;
        }

        public async Task<Page<TblCategory>> GetAllCategory(PaginationRequestModel pagingModel)
        {
            return await _context.TblCategories.PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
        }

        public async Task<Page<TblCategory>> GetCategoryByStatus(PaginationRequestModel pagingModel, string status)
        {
            return await _context.TblCategories.Where(x => x.Status == status).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
        }

        public async Task<TblCategory> selectDetailCategory(Guid categoryId)
        {
            return await _context.TblCategories.Where(x => x.Id == categoryId).FirstOrDefaultAsync();
        }

        public bool checkCategoryNameExist(string categoryName)
        {
            TblCategory? result = _context.TblCategories.Where(x => x.Name.Contains(categoryName)).FirstOrDefault();
            return result != null;
        }
        public bool checkCategoryIDExist(Guid categoryID)
        {
            TblCategory? result = _context.TblCategories.Where(x => x.Id.Equals(categoryID)).FirstOrDefault();
            return result != null;
        }
        public async Task<TblCategory> updateCategory(CategoryUpdateModel categoryUpdateModel)
        {
            //var category = await _context.TblCategories.Where(x =>x.Id == categoryUpdateModel.ID).FirstAsync();

            var query = from cate in context.TblCategories
                        where cate.Id.Equals(categoryUpdateModel.ID)
                        select new { cate };

            TblCategory? category = await query.Select(x => x.cate).FirstOrDefaultAsync();
            if (category == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(categoryUpdateModel.Name) && !categoryUpdateModel.Name.Equals(category.Name))
            {
                category.Name = categoryUpdateModel.Name;
            }
            if (!string.IsNullOrEmpty(categoryUpdateModel.Description) && !categoryUpdateModel.Description.Equals(category.Description))
            {
                category.Description = categoryUpdateModel.Description;
            }
            if (!string.IsNullOrEmpty(categoryUpdateModel.Status) && !categoryUpdateModel.Status.Equals(category.Status))
            {
                category.Status = categoryUpdateModel.Status;
            }


            _ = _context.Update(category);
            _ = await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> updateStatus(CategoryUpdateStatusModel model)
        {
            var result = await _context.TblCategories.Where(x=>x.Id.Equals(model.CategoryID)).FirstOrDefaultAsync();
            result.Status = model.status;
            _context.TblCategories.Update(result);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
