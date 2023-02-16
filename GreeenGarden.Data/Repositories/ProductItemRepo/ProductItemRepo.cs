using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public class ProductItemRepo : Repository<TblProductItem>, IProductItemRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ProductItemRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public Page<TblProductItem> queryAllItemByProductSize(PaginationRequestModel model, Guid productSizeId)
        {
            return _context.TblProductItems.Where(x=>x.SubProductId == productSizeId && x.Status == Status.ACTIVE).Paginate(model.curPage, model.pageSize);
        }

        public IQueryable<TblProductItem> queryDetailItemByProductSize(Guid productItemId)
        {
            return _context.TblProductItems.Where(x => x.Id == productItemId && x.Status == Status.ACTIVE);
        }

        public Page<TblSubProduct> queryAllSizeByProduct(PaginationRequestModel model, Guid productId)
        {
            return _context.TblSubProducts.Where(x => x.ProductId == productId).Paginate(model.curPage, model.pageSize);
        }

        public List<string> getImgByProductItem(Guid productItemId)
        {
            var listResult = _context.TblImages.Where(x=>x.ProductItemId == productItemId).ToList();
            if (listResult == null) return null;
            var listImg = new List<string>();
            foreach (var i in listResult)
            {
                listImg.Add(i.ImageUrl);
            }
            return listImg;
        }
    }
}
