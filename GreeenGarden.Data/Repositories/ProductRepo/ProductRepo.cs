using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductRepo
{
    public class ProductRepo : IProductRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ProductRepo(/*IMapper mapper,*/ GreenGardenDbContext context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public Page<TblProduct> queryAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId)
        {
            return _context.TblProducts.Where(x => x.Status == "Active"
                && x.CategoryId == categoryId
                && x.Quantity > 0).Paginate(pagingModel.curPage, pagingModel.pageSize);
        }
    }
}
