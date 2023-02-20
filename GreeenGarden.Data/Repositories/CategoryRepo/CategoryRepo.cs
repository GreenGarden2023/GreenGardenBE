using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Page<TblCategory> queryAllCategories(PaginationRequestModel pagingModel)
        {
            return _context.TblCategories.Where(x => x.Status == Status.ACTIVE).Paginate(pagingModel.curPage, pagingModel.pageSize);
        }

        public TblCategory selectDetailCategory(Guid categoryId)
        {
            return _context.TblCategories.Where(x => x.Id == categoryId).FirstOrDefault();
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
            if (!String.IsNullOrEmpty(categoryUpdateModel.Name))
            {
                category.Name = categoryUpdateModel.Name;
            }
            if (!String.IsNullOrEmpty(categoryUpdateModel.Description))
            {
                category.Description = categoryUpdateModel.Description;
            }
            if (!String.IsNullOrEmpty(categoryUpdateModel.Status)) category.Status = categoryUpdateModel.Status;


             _context.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
