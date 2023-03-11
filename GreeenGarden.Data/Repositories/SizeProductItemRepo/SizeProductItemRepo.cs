using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
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
            if (String.IsNullOrEmpty(status))
            {
                var result = await _context.TblProductItemDetails.Where(x => x.ProductItemId.Equals(ProductItemID)).ToListAsync();
                List<ProductItemDetailResModel> listSizeProd = new List<ProductItemDetailResModel>();
                foreach (TblProductItemDetail item in result)
                {
                    var sizeGet = await _sizeRepo.Get(item.SizeId);
                    var imgGet = await _imageRepo.GetImgUrlProductItemDetail(item.Id);
                    if (sizeGet != null)
                    {
                        var size = new SizeResModel()
                        {
                            Id = sizeGet.Id,
                            SizeName = sizeGet.Name
                        };
                        var sizeProd = new ProductItemDetailResModel
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
                var result = await _context.TblProductItemDetails.Where(x => x.ProductItemId.Equals(ProductItemID) && x.Status.Trim().ToLower().Equals(status.Trim().ToLower())).ToListAsync();
                List<ProductItemDetailResModel> listSizeProd = new List<ProductItemDetailResModel>();
                foreach (TblProductItemDetail item in result)
                {
                    var sizeGet = await _sizeRepo.Get(item.SizeId);
                    var imgGet = await _imageRepo.GetImgUrlProductItemDetail(item.Id);
                    if (sizeGet != null && imgGet != null)
                    {
                        var size = new SizeResModel()
                        {
                            Id = sizeGet.Id,
                            SizeName = sizeGet.Name
                        };
                        var sizeProd = new ProductItemDetailResModel
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

                var sizeProductItem = await query.Select(x => x.sizeProdItem).FirstOrDefaultAsync();
                if (sizeProductItem == null)
                {
                    return false;
                }
                if ((productItemDetailModel.SizeId != Guid.Empty) && !productItemDetailModel.SizeId.Equals(sizeProductItem.SizeId))
                {
                    sizeProductItem.SizeId = (Guid)productItemDetailModel.SizeId;
                }
                if ((productItemDetailModel.ProductItemID != Guid.Empty) && !productItemDetailModel.ProductItemID.Equals(sizeProductItem.ProductItemId))
                {
                    sizeProductItem.ProductItemId = (Guid)productItemDetailModel.ProductItemID;
                }
                if (!String.IsNullOrEmpty(productItemDetailModel.Status) && !productItemDetailModel.Status.Equals(sizeProductItem.Status))
                {
                    sizeProductItem.Status = productItemDetailModel.Status;
                }
                if (productItemDetailModel.RentPrice != null && productItemDetailModel.RentPrice != sizeProductItem.RentPrice)
                {
                    sizeProductItem.RentPrice = productItemDetailModel.RentPrice;
                }
                if (productItemDetailModel.SalePrice != null && productItemDetailModel.SalePrice != sizeProductItem.SalePrice)
                {
                    sizeProductItem.SalePrice = productItemDetailModel.SalePrice;
                }
                if (productItemDetailModel.Quantity != null && productItemDetailModel.Quantity != sizeProductItem.Quantity)
                {
                    sizeProductItem.Quantity = productItemDetailModel.Quantity;
                }
                _context.Update(sizeProductItem);
                await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }

        }
    }
}

