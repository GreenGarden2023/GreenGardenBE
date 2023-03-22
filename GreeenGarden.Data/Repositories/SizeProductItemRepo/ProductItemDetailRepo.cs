using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
    public class ProductItemDetailRepo : Repository<TblProductItemDetail>, IProductItemDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly ISizeRepo _sizeRepo;
        private readonly IImageRepo _imageRepo;
        public ProductItemDetailRepo(GreenGardenDbContext context, ISizeRepo sizeRepo, IImageRepo imageRepo) : base(context)
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
                            SizeName = sizeGet.Name,
                            SizeType = sizeGet.Type
                        };
                        ProductItemDetailResModel sizeProd = new()
                        {
                            Id = item.Id,
                            Size = size,
                            RentPrice = item.RentPrice,
                            SalePrice = item.SalePrice,
                            Quantity = item.Quantity,
                            TransportFee = item.TransportFee,
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
                            SizeName = sizeGet.Name,
                            SizeType = sizeGet.Type
                        };
                        ProductItemDetailResModel sizeProd = new()
                        {
                            Id = item.Id,
                            Size = size,
                            RentPrice = item.RentPrice,
                            SalePrice = item.SalePrice,
                            Quantity = item.Quantity,
                            Status = item.Status,
                            TransportFee = item.TransportFee,
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
            if (productItemDetailModel != null)
            {
                var query = from sizeProdItem in context.TblProductItemDetails
                            where sizeProdItem.Id.Equals(productItemDetailModel.Id)
                            select new { sizeProdItem };

                TblProductItemDetail? productItemDetail = await query.Select(x => x.sizeProdItem).FirstOrDefaultAsync();
                if (productItemDetail == null)
                {
                    return false;
                }

                productItemDetail.SizeId = productItemDetailModel.SizeId;

                productItemDetail.ProductItemId = productItemDetailModel.ProductItemID;

                productItemDetail.Status = productItemDetailModel.Status;

                productItemDetail.RentPrice = productItemDetailModel.RentPrice;

                productItemDetail.SalePrice = productItemDetailModel.SalePrice;

                productItemDetail.Quantity = productItemDetailModel.Quantity;

                productItemDetail.TransportFee = productItemDetailModel.TransportFee;

                _ = _context.Update(productItemDetail);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }

        }

        public async Task<bool> UpdateProductItemDetailQuantity(Guid productItemDetailID, int deductQuantity)
        {
            TblProductItemDetail result = await _context.TblProductItemDetails.Where(x => x.Id.Equals(productItemDetailID)).FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }
            result.Quantity -= deductQuantity;
            _ = _context.Update(result);
            _ = await _context.SaveChangesAsync();
            return true;
        }
    }
}

