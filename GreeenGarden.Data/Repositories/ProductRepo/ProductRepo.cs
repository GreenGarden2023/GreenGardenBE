using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductRepo
{
    public class ProductRepo : Repository<TblProduct>, IProductRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ProductRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
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

        public async void increaseQuantity(Guid subId, int plus)
        {
            /*var subPro = await _context.TblSubProducts.Where(x => x.Id == subId).FirstAsync();
            var result = await _context.TblProducts.Where(x => x.Equals(subPro.ProductId)).FirstAsync();
            result.Quantity = result.Quantity + plus;
            _context.Update(result);
            await _context.SaveChangesAsync();*/
        }

        public Page<TblProduct> queryAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId)
        {
            return _context.TblProducts.Where(x => x.Status == Status.ACTIVE
                && x.CategoryId == categoryId
                && x.Quantity > 0).Paginate(pagingModel.curPage, pagingModel.pageSize);
        }

        public TblProduct queryAProductByProId(Guid? proId)
        {
            return _context.TblProducts.Where(x => x.Id == proId).FirstOrDefault();
        }

        public void updateProduct(TblProduct product)
        {
            _context.TblProducts.Update(product);
            _context.SaveChangesAsync();
        }
    }
}
