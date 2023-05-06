using Azure.Core;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;
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

        public async Task<bool> ChangeStatus(ProductItemDetailUpdateStatusModel model)
        {
            TblProductItemDetail? result = await _context.TblProductItemDetails.Where(x => x.Id.Equals(model.ProductItemDetailId)).FirstOrDefaultAsync();
            result.Status = model.Status;
            _ = _context.TblProductItemDetails.Update(result);
            _ = await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Page<TblProductItem>> GetProductItemByType(PaginationRequestModel paginationRequestModel, Guid productID, string? type)
        {
            var result = new Page<TblProductItem>();

            var listResult = new List<TblProductItem>();
            var listResultCop = new List<TblProductItem>();
            if (string.IsNullOrEmpty(type))
            {
                listResult = await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)).ToListAsync();
            }
            else
            {
                listResult = await _context.TblProductItems.Where(x => x.ProductId.Equals(productID)&&x.Type.Equals(type)).ToListAsync();
            }
            if (listResult == null) return null;
            foreach (var a in listResult)
            {
                var quantity = 0;
                var proItemDetail = await _context.TblProductItemDetails.Where(x => x.ProductItemId.Equals(a.Id) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                if (proItemDetail.Any())
                {
                    foreach (var b in proItemDetail)
                    {
                        quantity += (int)b.Quantity;
                    }
                    if (quantity != 0)
                    {
                        listResultCop.Add(a);
                    }
                }
            }
            if (listResultCop == null) return null;

            var listResultPaging = listResultCop.Skip((paginationRequestModel.curPage - 1) * paginationRequestModel.pageSize).Take(paginationRequestModel.pageSize);

            result.PageSize = paginationRequestModel.pageSize;
            result.CurrentPage = paginationRequestModel.curPage;
            result.RecordCount = listResultCop.Count();
            result.PageCount = (int)Math.Ceiling((double)result.RecordCount / result.PageSize);

            result.Results = listResultPaging.ToList();

            return result;

        }

        public async Task<Page<TblProductItem>> GetProductItemByTypeByManager(PaginationRequestModel paginationRequestModel, Guid productId, string? type)
        {
            var result = new Page<TblProductItem>();

            var listResult = new List<TblProductItem>();
            if (string.IsNullOrEmpty(type))
            {
                listResult = await _context.TblProductItems.Where(x => x.ProductId.Equals(productId)).ToListAsync();
            }
            else
            {
                listResult = await _context.TblProductItems.Where(x => x.ProductId.Equals(productId) && x.Type.Equals(type)).ToListAsync();
            }
            if (listResult == null) return null;
            var listResultPaging = listResult.Skip((paginationRequestModel.curPage - 1) * paginationRequestModel.pageSize).Take(paginationRequestModel.pageSize);

            result.PageSize = paginationRequestModel.pageSize;
            result.CurrentPage = paginationRequestModel.curPage;
            result.RecordCount = listResult.Count();
            result.PageCount = (int)Math.Ceiling((double)result.RecordCount / result.PageSize);

            result.Results = listResultPaging.ToList();

            return result;
        }

        public async Task<Page<TblProductItem>> searchProductItem(Guid productID, PaginationRequestModel pagingModel)
        {
            var product = await _context.TblProducts.Where(x => x.Id == productID).FirstOrDefaultAsync();
            if (product == null) return null;
            return await _context.TblProductItems.Where(x => x.ProductId.Equals(product.Id) && x.Name.Contains(pagingModel.searchText)).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
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
                if (!string.IsNullOrEmpty(productItemModel.Rule) && !productItemModel.Rule.Equals(productItem.Rule))
                {
                    productItem.Rule = productItemModel.Rule;
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
