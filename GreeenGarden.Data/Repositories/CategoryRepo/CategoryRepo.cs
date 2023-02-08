using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.CategoryRepo
{
    public class CategoryRepo : ICategoryRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public CategoryRepo(/*IMapper mapper,*/ GreenGardenDbContext context)
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
            return _context.TblCategories.Where(x => x.Status == "Active").Paginate(pagingModel.curPage, pagingModel.pageSize);
        }
    }
}
