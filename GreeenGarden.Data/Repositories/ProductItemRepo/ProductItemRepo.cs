using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public class ProductItemRepo : Repository<TblProductItem>, IProductItemRepo
    {
        private readonly GreenGardenDbContext _context;
        public ProductItemRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Page<TblProductItem>> GetProductItemByType(PaginationRequestModel paginationRequestModel, Guid productID, string? type)
        {
            return string.IsNullOrEmpty(type)
                ? await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize)
                : await _context.TblProductItems.Where(x => x.Type.Trim().ToLower().Equals(type.Trim().ToLower()) && x.ProductId.Equals(productID)).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
        }

        public async Task<bool> UpdateProductItem(ProductItemModel productItemModel)
        {
            if (productItemModel.ProductId == null)
            {
                productItemModel.ProductId = Guid.Empty;
            }
            if (productItemModel != null)
            {
                var query = from prodItem in context.TblProductItems
                            where prodItem.Id.Equals(productItemModel.Id)
                            select new { prodItem };

                TblProductItem? productItem = await query.Select(x => x.prodItem).FirstOrDefaultAsync();
                if (productItem == null)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(productItemModel.Name) && !productItemModel.Name.Equals(productItem.Name))
                {
                    productItem.Name = productItemModel.Name;
                }
                if (!string.IsNullOrEmpty(productItemModel.Description) && !productItemModel.Description.Equals(productItem.Description))
                {
                    productItem.Description = productItemModel.Description;
                }
                if (!string.IsNullOrEmpty(productItemModel.Type) && !productItemModel.Type.Equals(productItem.Type))
                {
                    productItem.Type = productItemModel.Type;
                }
                if (!string.IsNullOrEmpty(productItemModel.Content) && !productItemModel.Content.Equals(productItem.Content))
                {
                    productItem.Content = productItemModel.Content;
                }
                if ((productItemModel.ProductId != Guid.Empty) && !productItemModel.ProductId.Equals(productItem.ProductId))
                {
                    productItem.ProductId = (Guid)productItemModel.ProductId;
                }
                _ = context.Update(productItem);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }
    }

}
