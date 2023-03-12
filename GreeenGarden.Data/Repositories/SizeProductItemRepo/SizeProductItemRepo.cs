using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.Repository;
using GreeenGarden.Data.Repositories.SizeRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
    public class SizeProductItemRepo : Repository<TblProductItemDetail>, ISizeProductItemRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly ISizeRepo _sizeRepo;
        private readonly IImageRepo _imageRepo;
        public SizeProductItemRepo(GreenGardenDbContext context, ISizeRepo sizeRepo, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _sizeRepo = sizeRepo;
            _imageRepo = imageRepo;
        }

        public async Task<List<ProductItemDetailResModel>> GetSizeProductItems(Guid ProductItemID, string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                List<TblProductItemDetail> result = await _context.TblProductItemDetails.Where(x => x.ProductItemId.Equals(ProductItemID)).ToListAsync();
                List<ProductItemDetailResModel> listSizeProd = new();
                foreach (TblProductItemDetail item in result)
                {
                    TblSize? sizeGet = await _sizeRepo.Get(item.SizeId);
                    List<string> imgGet = await _imageRepo.GetImgUrlProductItemDetail(item.Id);
                    if (sizeGet != null)
                    {
                        SizeResModel size = new()
                        {
                            Id = sizeGet.Id,
                            SizeName = sizeGet.Name
                        };
                        ProductItemDetailResModel sizeProd = new()
                        {
                            Id = item.Id,
                            Size = size,
                            RentPrice = item.RentPrice,
                            SalePrice = item.SalePrice,
                            Quantity = item.Quantity,
                            Status = item.Status,
                            ImagesURL = imgGet
                        };
                        listSizeProd.Add(sizeProd);
                    }

                }
                return listSizeProd;
            }
            else
            {
                List<TblProductItemDetail> result = await _context.TblProductItemDetails.Where(x => x.ProductItemId.Equals(ProductItemID) && x.Status.Trim().ToLower().Equals(status.Trim().ToLower())).ToListAsync();
                List<ProductItemDetailResModel> listSizeProd = new();
                foreach (TblProductItemDetail item in result)
                {
                    TblSize? sizeGet = await _sizeRepo.Get(item.SizeId);
                    List<string> imgGet = await _imageRepo.GetImgUrlProductItemDetail(item.Id);
                    if (sizeGet != null && imgGet != null)
                    {
                        SizeResModel size = new()
                        {
                            Id = sizeGet.Id,
                            SizeName = sizeGet.Name
                        };
                        ProductItemDetailResModel sizeProd = new()
                        {
                            Id = item.Id,
                            Size = size,
                            RentPrice = item.RentPrice,
                            SalePrice = item.SalePrice,
                            Quantity = item.Quantity,
                            Status = item.Status,
                            ImagesURL = imgGet,
                        };
                        listSizeProd.Add(sizeProd);
                    }

                }
                return listSizeProd;
            }
        }

        public async Task<bool> UpdateSizeProductItem(ProductItemDetailModel productItemDetailModel)
        {
            if (productItemDetailModel.SizeId == null)
            {
                productItemDetailModel.SizeId = Guid.Empty;
            }
            if (productItemDetailModel.ProductItemID == null)
            {
                productItemDetailModel.ProductItemID = Guid.Empty;
            }
            if (productItemDetailModel != null)
            {
                var query = from sizeProdItem in context.TblProductItemDetails
                            where sizeProdItem.Id.Equals(productItemDetailModel.Id)
                            select new { sizeProdItem };

                TblProductItemDetail? sizeProductItem = await query.Select(x => x.sizeProdItem).FirstOrDefaultAsync();
                if (sizeProductItem == null)
                {
                    return false;
                }
                
                    sizeProductItem.SizeId = productItemDetailModel.SizeId;
                
                    sizeProductItem.ProductItemId = productItemDetailModel.ProductItemID;
                
                    sizeProductItem.Status = productItemDetailModel.Status;
                
                    sizeProductItem.RentPrice = productItemDetailModel.RentPrice;
                
                    sizeProductItem.SalePrice = productItemDetailModel.SalePrice;
                
                    sizeProductItem.Quantity = productItemDetailModel.Quantity;
                
                _ = _context.Update(sizeProductItem);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }

        }
    }
}

