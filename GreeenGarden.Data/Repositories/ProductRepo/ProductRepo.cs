using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

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
                && x.CategoryId == categoryId).Paginate(pagingModel.curPage, pagingModel.pageSize);
        }

        public Page<TblProduct> queryAllProduct(PaginationRequestModel pagingModel)
        {
            return _context.TblProducts.Paginate(pagingModel.curPage, pagingModel.pageSize);
        }

        public TblProduct queryAProductByProId(Guid? proId)
        {
            return _context.TblProducts.Where(x => x.Id == proId).FirstOrDefault();
        }

        public async Task<bool> UpdateProduct(ProductUpdateModel productUpdateModel)
        {

            var query = from prod in context.TblProducts
                        where prod.Id.Equals(productUpdateModel.ID)
                        select new { prod };

            var product = await query.Select(x => x.prod).FirstOrDefaultAsync();
            if (product == null)
            {
                return false;
            }
            if (!String.IsNullOrEmpty(productUpdateModel.Name) && !productUpdateModel.Name.Equals(product.Name))
            {
                product.Name = productUpdateModel.Name;
            }
            if (!String.IsNullOrEmpty(productUpdateModel.Description) && !productUpdateModel.Description.Equals(product.Description))
            {
                product.Description = productUpdateModel.Description;
            }
            if (!String.IsNullOrEmpty(productUpdateModel.Status) && !productUpdateModel.Status.Equals(product.Status))
            {
                product.Status = productUpdateModel.Status;
            }
            if (productUpdateModel.CategoryId != Guid.Empty && !productUpdateModel.CategoryId.Equals(product.CategoryId))
            {
                product.CategoryId = productUpdateModel.CategoryId;
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
