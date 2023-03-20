using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public class ImageRepo : Repository<TblImage>, IImageRepo
    {
        private readonly GreenGardenDbContext _context;
        public ImageRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> DeleteImage(string imgURL)
        {
            if (!string.IsNullOrEmpty(imgURL))
            {
                TblImage? img = _context.TblImages.Where(x => x.ImageUrl == imgURL).FirstOrDefault();
                if (img != null)
                {
                    _ = _context.TblImages.Remove(img);
                    await Update();
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }

        public async Task<TblImage> GetImgUrlCategory(Guid categoryId)
        {
            var query = from c in context.TblImages
                        where c.CategoryId.Equals(categoryId)
                        select new { c };
            TblImage? result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                CategoryId = x.c.CategoryId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TblImage> GetImgUrlProduct(Guid productID)
        {
            var query = from c in context.TblImages
                        where c.ProductId.Equals(productID)
                        select new { c };
            TblImage? result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TblImage> GetImgUrlProductItem(Guid productItemID)
        {
            var query = from c in context.TblImages
                        where c.ProductItemId.Equals(productItemID)
                        select new { c };
            TblImage? result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<string>> GetImgUrlProductItemDetail(Guid productItemDetailID)
        {
            List<string> urls = new();
            if (productItemDetailID != Guid.Empty)
            {

                List<TblImage> result = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(productItemDetailID)).ToListAsync();
                if (result != null)
                {

                    foreach (TblImage image in result)
                    {
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            urls.Add(image.ImageUrl);
                        }
                    }

                }
            }
            return urls;
        }

        public async Task<TblImage> GetImgUrlRentOrderDetail(Guid rentOrderDetailID)
        {
            var query = from c in context.TblImages
                        where c.RentOrderDetailId.Equals(rentOrderDetailID)
                        select new { c };
            TblImage? result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TblImage> GetImgUrlSaleOrderDetail(Guid saleOrderDetailID)
        {
            var query = from c in context.TblImages
                        where c.SaleOrderDetailId.Equals(saleOrderDetailID)
                        select new { c };
            TblImage? result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<string>> GetImgUrlUserTree(Guid userTreeID)
        {
            List<string> urls = new();
            if (userTreeID != Guid.Empty)
            {

                List<TblImage> result = await _context.TblImages.Where(x => x.UserTreeId.Equals(userTreeID)).ToListAsync();
                if (result != null)
                {

                    foreach (TblImage image in result)
                    {
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            urls.Add(image.ImageUrl);
                        }
                    }

                }
            }
            return urls;
        }

        public async Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl)
        {
            TblImage? imgCategory = await _context.TblImages.Where(x => x.CategoryId == categoryId).FirstOrDefaultAsync();
            if (imgCategory != null)
            {
                imgCategory.ImageUrl = imgUrl;
                _ = _context.Update(imgCategory);
                _ = await _context.SaveChangesAsync();
            }

            return imgCategory;
        }

        public async Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl)
        {
            TblImage? imgProduct = await _context.TblImages.Where(x => x.ProductId.Equals(ProductID)).FirstOrDefaultAsync();
            if (imgProduct != null)
            {
                imgProduct.ImageUrl = ImgUrl;
                _ = _context.Update(imgProduct);
                _ = await _context.SaveChangesAsync();
                return imgProduct;
            }
            else
            {
                TblImage newProdIMG = new()
                {
                    ImageUrl = ImgUrl,
                    ProductId = ProductID
                };
                _ = await _context.TblImages.AddAsync(newProdIMG);
                _ = await _context.SaveChangesAsync();
                return newProdIMG;
            }

        }

        public async Task<TblImage> UpdateImgForProductItem(Guid ProductItemID, string ImgUrl)
        {
            TblImage? imgProduct = await _context.TblImages.Where(x => x.ProductItemId.Equals(ProductItemID)).FirstOrDefaultAsync();
            if (imgProduct != null)
            {
                imgProduct.ImageUrl = ImgUrl;
                _ = _context.Update(imgProduct);
                _ = await _context.SaveChangesAsync();
                return imgProduct;
            }
            else
            {
                TblImage newProdIMG = new()
                {
                    ImageUrl = ImgUrl,
                    ProductItemId = ProductItemID
                };
                _ = await _context.TblImages.AddAsync(newProdIMG);
                _ = await _context.SaveChangesAsync();
                return newProdIMG;
            }
        }

        public async Task<bool> UpdateImgForProductItemDetail(Guid ProductItemDetailId, List<string> ImgUrls)
        {
            bool success = false;
            List<TblImage> oldImgList = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(ProductItemDetailId)).ToListAsync();
            foreach (TblImage tblImage in oldImgList)
            {
                try
                {
                    _ = _context.Remove(tblImage);
                    _ = await _context.SaveChangesAsync();
                    success = true;
                }
                catch
                {
                    success = false;
                    return success;
                }
            }
            foreach (string url in ImgUrls)
            {
                try
                {
                    TblImage newProdIMG = new()
                    {
                        ImageUrl = url,
                        ProductItemDetailId = ProductItemDetailId
                    };
                    _ = _context.Add(newProdIMG);
                    _ = await _context.SaveChangesAsync();
                    success = true;
                }
                catch
                {
                    success = false;
                    return success;
                }
            }
            return success;
        }

        public async Task<bool> UpdateImgForUserTree(Guid userTreeID, List<string> ImgUrls)
        {
            bool success = false;
            List<TblImage> oldImgList = await _context.TblImages.Where(x => x.UserTreeId.Equals(userTreeID)).ToListAsync();
            foreach (TblImage tblImage in oldImgList)
            {
                try
                {
                    _ = _context.Remove(tblImage);
                    _ = await _context.SaveChangesAsync();
                    success = true;
                }
                catch
                {
                    success = false;
                    return success;
                }
            }
            foreach (string url in ImgUrls)
            {
                try
                {
                    TblImage newProdIMG = new()
                    {
                        ImageUrl = url,
                        UserTreeId = userTreeID
                    };
                    _ = _context.Add(newProdIMG);
                    _ = await _context.SaveChangesAsync();
                    success = true;
                }
                catch
                {
                    success = false;
                    return success;
                }
            }
            return success;
        }
    }
}
