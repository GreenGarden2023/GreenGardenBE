using System;
using GreeenGarden.Data.Entities;
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

        public async Task<List<SizeProductItemResModel>> GetSizeProductItems(Guid productItemId, string status)
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
    }
}

