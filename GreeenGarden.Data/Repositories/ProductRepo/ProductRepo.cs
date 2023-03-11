using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
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


        public async Task<Page<TblProduct>> queryAllProductByCategoryAndStatus(PaginationRequestModel pagingModel, Guid categoryID, string? status, string? rentSale)
        {
            if (!String.IsNullOrEmpty(rentSale) && rentSale.Trim().ToLower().Equals("sale") && !String.IsNullOrEmpty(status))
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
                && x.Status.Trim().ToLower().Equals(status.Trim().ToLower())
                && x.IsForSale == true).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else if (!String.IsNullOrEmpty(rentSale) && rentSale.Trim().ToLower().Equals("rent") && !String.IsNullOrEmpty(status))
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
                && x.IsForRent == true
                && x.Status.Trim().ToLower().Equals(status)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else if (!String.IsNullOrEmpty(rentSale) && rentSale.Trim().ToLower().Equals("sale") && String.IsNullOrEmpty(status))
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
                && x.IsForSale == true).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else if (!String.IsNullOrEmpty(rentSale) && rentSale.Trim().ToLower().Equals("rent") && String.IsNullOrEmpty(status))
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
                && x.IsForRent == true
                ).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else if (String.IsNullOrEmpty(rentSale) && !String.IsNullOrEmpty(status))
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
                && x.Status.Trim().ToLower().Equals(status)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else
            {
                return await _context.TblProducts.Where(x => x.CategoryId.Equals(categoryID)
               ).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
        }


        public async Task<Page<TblProduct>> queryAllProduct(PaginationRequestModel pagingModel)
        {
            return await _context.TblProducts.PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
        }


        public async Task<TblProduct> queryAProductByProId(Guid? proId)
        {
            return await _context.TblProducts.Where(x => x.Id == proId).FirstOrDefaultAsync();
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
            if (!productUpdateModel.IsForRent.Equals(product.IsForRent))
            {
                product.IsForRent = productUpdateModel.IsForRent;
            }
            if (!productUpdateModel.IsForSale.Equals(product.IsForSale))
            {
                product.IsForSale = productUpdateModel.IsForSale;
            }


            _context.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
