using System.Linq;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> UpdateProductItem(ProductItemUpdateModel productItemUpdateModel)
        {
            var query = from prod in context.TblProductItems
                        where prod.Id.Equals(productItemUpdateModel.Id)
                        select new { prod };
            var product = await query.Select(x => x.prod).FirstOrDefaultAsync();
            if (product == null)
            {
                return false;
            }
            if (!String.IsNullOrEmpty(productItemUpdateModel.Name) && !productItemUpdateModel.Name.Equals(product.Name))
            {
                product.Name = productItemUpdateModel.Name;
            }
            if (productItemUpdateModel.SalePrice != null && productItemUpdateModel.SalePrice != 0 && !productItemUpdateModel.SalePrice.Equals(product.SalePrice))
            {
                product.SalePrice = productItemUpdateModel.SalePrice;
            }
            if (!String.IsNullOrEmpty(productItemUpdateModel.Status) && !productItemUpdateModel.Status.Equals(product.Status))
            {
                product.Status = productItemUpdateModel.Status;
            }
            if (!String.IsNullOrEmpty(productItemUpdateModel.Description) && !productItemUpdateModel.Description.Equals(product.Description))
            {
                product.Description = productItemUpdateModel.Description;
            }
            if (productItemUpdateModel.ProductId != null && productItemUpdateModel.ProductId != Guid.Empty && !productItemUpdateModel.ProductId.Equals(product.ProductId))
            {
                product.ProductId = productItemUpdateModel.ProductId;
            }
            if (productItemUpdateModel.SizeId != Guid.Empty && productItemUpdateModel.SizeId != null && !productItemUpdateModel.SizeId.Equals(product.SizeId))
            {
                product.SizeId = (Guid)productItemUpdateModel.SizeId;
            }
            if (productItemUpdateModel.Quantity != null && productItemUpdateModel.Quantity != 0 && !productItemUpdateModel.Quantity.Equals(product.Quantity))
            {
                product.Quantity = productItemUpdateModel.Quantity;
            }
            if (!String.IsNullOrEmpty(productItemUpdateModel.Type) && !productItemUpdateModel.Type.Equals(product.Type))
            {
                product.Type = productItemUpdateModel.Type;
            }
            if (productItemUpdateModel.RentPrice != null && productItemUpdateModel.RentPrice != 0 && !productItemUpdateModel.RentPrice.Equals(product.RentPrice))
            {
                product.RentPrice = productItemUpdateModel.RentPrice;
            }
            if (!String.IsNullOrEmpty(productItemUpdateModel.Content) && !productItemUpdateModel.Content.Equals(product.Content))
            {
                product.Content = productItemUpdateModel.Content;
            }
            _context.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    
}
