using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
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

        public string getImgByProduct(Guid productId)
        {
            var result = _context.TblImages.Where(x => x.ProductId == productId).FirstOrDefault();
            if (result == null) return null;

            return result.ImageUrl;
        }

        public Page<TblProduct> queryAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId)
        {
            return _context.TblProducts.Where(x => x.Status == Status.ACTIVE
                && x.CategoryId == categoryId
                && x.Quantity > 0).Paginate(pagingModel.curPage, pagingModel.pageSize);
        }
    }
}
