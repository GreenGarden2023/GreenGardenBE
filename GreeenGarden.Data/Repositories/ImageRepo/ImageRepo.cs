﻿using GreeenGarden.Data.Entities;
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
            if (!String.IsNullOrEmpty(imgURL))
            {
                var img = _context.TblImages.Where(x => x.ImageUrl == imgURL).FirstOrDefault();
                if (img != null)
                {
                    _context.TblImages.Remove(img);
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
            var result = await query.Select(x => new TblImage()
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
            var result = await query.Select(x => new TblImage()
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
            var result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<string>> GetImgUrlProductItemDetail(Guid productItemDetailID)
        {
            List<string> urls = new List<string>();
            if (productItemDetailID != Guid.Empty)
            {

                var result = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(productItemDetailID)).ToListAsync();
                if (result != null)
                {

                    foreach (TblImage image in result)
                    {
                        if (!String.IsNullOrEmpty(image.ImageUrl))
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
            var imgCategory = await _context.TblImages.Where(x => x.CategoryId == categoryId).FirstOrDefaultAsync();
            if (imgCategory != null)
            {
                imgCategory.ImageUrl = imgUrl;
                _context.Update(imgCategory);
                await _context.SaveChangesAsync();
            }
             
            return imgCategory;
        }

        public async Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl)
        {
            var imgProduct = await _context.TblImages.Where(x => x.ProductId.Equals(ProductID)).FirstOrDefaultAsync();
            if (imgProduct != null)
            {
                imgProduct.ImageUrl = ImgUrl;
                _context.Update(imgProduct);
                await _context.SaveChangesAsync();
                return imgProduct;
            }
            else
            {
                var newProdIMG = new TblImage
                {
                    ImageUrl = ImgUrl,
                    ProductId = ProductID
                };
                await _context.TblImages.AddAsync(newProdIMG);
                await _context.SaveChangesAsync();
                return newProdIMG;
            }

        }

        public async Task<TblImage> UpdateImgForProductItem(Guid ProductItemID, string ImgUrl)
        {
            var imgProduct = await _context.TblImages.Where(x => x.ProductItemId.Equals(ProductItemID)).FirstOrDefaultAsync();
            if (imgProduct != null)
            {
                imgProduct.ImageUrl = ImgUrl;
                _context.Update(imgProduct);
                await _context.SaveChangesAsync();
                return imgProduct;
            }
            else
            {
                var newProdIMG = new TblImage
                {
                    ImageUrl = ImgUrl,
                    ProductItemId = ProductItemID
                };
                await _context.TblImages.AddAsync(newProdIMG);
                await _context.SaveChangesAsync();
                return newProdIMG;
            }
        }

        public async Task<bool> UpdateImgForProductItemDetail(Guid ProductItemDetailId, List<string> ImgUrls)
        {
            bool success = false;
            var oldImgList = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(ProductItemDetailId)).ToListAsync();
            foreach (TblImage tblImage in oldImgList)
            {
                try
                {
                    _context.Remove(tblImage);
                    await _context.SaveChangesAsync();
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
                    var newProdIMG = new TblImage
                    {
                        ImageUrl = url,
                        ProductItemDetailId = ProductItemDetailId
                    };
                    _context.Add(newProdIMG);
                    await _context.SaveChangesAsync();
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
