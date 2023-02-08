using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public class ProductItemRepo : IProductItemRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ProductItemRepo(/*IMapper mapper,*/ GreenGardenDbContext context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public Page<TblProductItem> queryAllItemByProductSize(PaginationRequestModel model, Guid productSizeId)
        {
            return _context.TblProductItems.Where(x=>x.SubProductId == productSizeId && x.Status == "Active").Paginate(model.curPage, model.pageSize);
        }

        public IQueryable<TblProductItem> queryDetailItemByProductSize(Guid productItemId)
        {
            return _context.TblProductItems.Where(x => x.Id == productItemId && x.Status == "Active");
        }

        public Page<TblSubProduct> queryAllSizeByProduct(PaginationRequestModel model, Guid productId)
        {
            return _context.TblSubProducts.Where(x => x.ProductId == productId).Paginate(model.curPage, model.pageSize);
        }
    }
}
