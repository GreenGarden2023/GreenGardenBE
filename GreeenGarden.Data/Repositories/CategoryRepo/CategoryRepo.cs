using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
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


        public string getImgByCategory(Guid categoryId)
        {
            var result = _context.TblImages.Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (result == null) return null;

            return result.ImageUrl;
        }

        public Page<TblCategory> GetAllCategory(PaginationRequestModel pagingModel)
        {
            return _context.TblCategories.Paginate(pagingModel.curPage, pagingModel.pageSize);
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
            var result = _context.TblCategories.Where(x => x.Name.Contains(categoryName)).FirstOrDefault();
            if (result != null) { return true; } else { return false; }
        }
        public bool checkCategoryIDExist(Guid categoryID)
        {
            var result = _context.TblCategories.Where(x => x.Id.Equals(categoryID)).FirstOrDefault();
            if (result != null) { return true; } else { return false; }
        }
        public async Task<TblCategory> updateCategory(CategoryUpdateModel categoryUpdateModel)
        {
            //var category = await _context.TblCategories.Where(x =>x.Id == categoryUpdateModel.ID).FirstAsync();

            var query = from cate in context.TblCategories
                        where cate.Id.Equals(categoryUpdateModel.ID)
                        select new { cate };

            var category = await query.Select(x => x.cate).FirstOrDefaultAsync();
            if (category == null)
            {
                return null;
            }
            if (!String.IsNullOrEmpty(categoryUpdateModel.Name) && !categoryUpdateModel.Name.Equals(category.Name))
            {
                category.Name = categoryUpdateModel.Name;
            }
            if (!String.IsNullOrEmpty(categoryUpdateModel.Description) && !categoryUpdateModel.Description.Equals(category.Description))
            {
                category.Description = categoryUpdateModel.Description;
            }
            if (!String.IsNullOrEmpty(categoryUpdateModel.Status) && !categoryUpdateModel.Status.Equals(category.Status))
            {
                category.Status = categoryUpdateModel.Status;
            }


            _context.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
