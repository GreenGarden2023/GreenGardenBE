using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
	public class SizeProductItemRepo : Repository<TblSizeProductItem>, ISizeProductItemRepo
	{
        private readonly GreenGardenDbContext _context;
        public SizeProductItemRepo(GreenGardenDbContext context) : base(context)
        {
			_context = context;
		}

        public async Task<List<SizeProductItemResModel>> GetSizeProductItems(Guid productItemId, string? status)
        {
            if (String.IsNullOrEmpty(status))
            {
                var result = await _context.TblSizeProductItems.Where(x => x.ProductItemId.Equals(productItemId)).ToListAsync();
                List<SizeProductItemResModel> listSizeProd = new List<SizeProductItemResModel>();
                foreach (TblSizeProductItem item in result)
                {
                    var sizeProd = new SizeProductItemResModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        SizeId = item.SizeId,
                        ProductItemId = item.ProductItemId,
                        RentPrice = item.RentPrice,
                        SalePrice = item.SalePrice,
                        Quantity = item.Quantity,
                        Content = item.Content,
                        Status = item.Status
                    };
                    listSizeProd.Add(sizeProd);
                }
                return listSizeProd;
            }
            else
            {
                var result = await _context.TblSizeProductItems.Where(x => x.ProductItemId.Equals(productItemId) && x.Status.Trim().ToLower().Equals(status.Trim().ToLower())).ToListAsync();
                List<SizeProductItemResModel> listSizeProd = new List<SizeProductItemResModel>();
                foreach (TblSizeProductItem item in result)
                {
                    var sizeProd = new SizeProductItemResModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        SizeId = item.SizeId,
                        ProductItemId = item.ProductItemId,
                        RentPrice = item.RentPrice,
                        SalePrice = item.SalePrice,
                        Quantity = item.Quantity,
                        Content = item.Content,
                        Status = item.Status
                    };
                    listSizeProd.Add(sizeProd);
                }
                return listSizeProd;
            }
        }

        public async Task<bool> UpdateSizeProductItem(SizeProductItemUpdateModel sizeProductItemUpdateModel)
        {
            if(sizeProductItemUpdateModel.SizeId == null)
            {
                sizeProductItemUpdateModel.SizeId = Guid.Empty;
            }
            if (sizeProductItemUpdateModel.ProductItemId == null)
            {
                sizeProductItemUpdateModel.ProductItemId = Guid.Empty;
            }
            if (sizeProductItemUpdateModel != null)
            {
                var query = from sizeProdItem in context.TblSizeProductItems
                            where sizeProdItem.Id.Equals(sizeProductItemUpdateModel.Id)
                            select new { sizeProdItem };

                var sizeProductItem = await query.Select(x => x.sizeProdItem).FirstOrDefaultAsync();
                if (sizeProductItem == null)
                {
                    return false;
                }
                if ((sizeProductItemUpdateModel.SizeId != Guid.Empty) && !sizeProductItemUpdateModel.SizeId.Equals(sizeProductItem.SizeId))
                {
                    sizeProductItem.SizeId = (Guid)sizeProductItemUpdateModel.SizeId;
                }
                if ((sizeProductItemUpdateModel.ProductItemId != Guid.Empty) && !sizeProductItemUpdateModel.ProductItemId.Equals(sizeProductItem.ProductItemId))
                {
                    sizeProductItem.ProductItemId = (Guid)sizeProductItemUpdateModel.ProductItemId;
                }
                if (!String.IsNullOrEmpty(sizeProductItemUpdateModel.Name) && !sizeProductItemUpdateModel.Name.Equals(sizeProductItem.Name))
                {
                    sizeProductItem.Name = sizeProductItemUpdateModel.Name;
                }
                if (!String.IsNullOrEmpty(sizeProductItemUpdateModel.Content) && !sizeProductItemUpdateModel.Content.Equals(sizeProductItem.Content))
                {
                    sizeProductItem.Content = sizeProductItemUpdateModel.Content;
                }
                if (!String.IsNullOrEmpty(sizeProductItemUpdateModel.Status) && !sizeProductItemUpdateModel.Status.Equals(sizeProductItem.Status))
                {
                    sizeProductItem.Status = sizeProductItemUpdateModel.Status;
                }
                if (sizeProductItemUpdateModel.RentPrice != null && sizeProductItemUpdateModel.RentPrice != sizeProductItem.RentPrice)
                {
                    sizeProductItem.RentPrice = sizeProductItemUpdateModel.RentPrice;
                }
                if (sizeProductItemUpdateModel.SalePrice != null && sizeProductItemUpdateModel.SalePrice != sizeProductItem.SalePrice)
                {
                    sizeProductItem.SalePrice = sizeProductItemUpdateModel.SalePrice;
                }
                if (sizeProductItemUpdateModel.Quantity != null && sizeProductItemUpdateModel.Quantity != sizeProductItem.Quantity)
                {
                    sizeProductItem.Quantity = sizeProductItemUpdateModel.Quantity;
                }
                _context.Update(sizeProductItem);
                await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }

        }
    }
}

