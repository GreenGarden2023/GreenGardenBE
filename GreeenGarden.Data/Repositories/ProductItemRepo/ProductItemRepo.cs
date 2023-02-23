using System.Linq;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public class ProductItemRepo : Repository<TblProductItem>, IProductItemRepo
    {
        private readonly GreenGardenDbContext _context;
        public ProductItemRepo( GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Page<TblProductItem>> GetProductItems(PaginationRequestModel pagingModel, Guid productID, Guid? sizeID, string? type, string? status)
        {
            if (sizeID == Guid.Empty  && String.IsNullOrEmpty(type) && String.IsNullOrEmpty(status))
            {
                
                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID != Guid.Empty && !String.IsNullOrEmpty(type) && String.IsNullOrEmpty(status))
            {
                
                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.SizeId.Equals(sizeID)
                && x.Type.Equals(type)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID != Guid.Empty && String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(status))
            {

                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.SizeId.Equals(sizeID)
                && x.Status.Trim().ToLower().Equals(status)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID != Guid.Empty && String.IsNullOrEmpty(type) && String.IsNullOrEmpty(status))
            {

                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.SizeId.Equals(sizeID)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID == Guid.Empty  && !String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(status))
            {

                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.Type.Equals(type)
                && x.Status.Trim().ToLower().Equals(status)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID == Guid.Empty  && !String.IsNullOrEmpty(type) && String.IsNullOrEmpty(status))
            {
                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.Type.Equals(type)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else if (sizeID == Guid.Empty  && String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(status))
            {
                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                 && x.Status.Trim().ToLower().Equals(status)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            ///
            else {
                return await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)
                && x.SizeId.Equals(sizeID)
                && x.Status.Trim().ToLower().Equals(status)
                && x.Type.Equals(type)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }

        }

    }
    
}
