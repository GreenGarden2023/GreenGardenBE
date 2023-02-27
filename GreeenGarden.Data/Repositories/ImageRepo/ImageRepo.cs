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
            if (!String.IsNullOrEmpty(imgURL))
            {
                var img = _context.TblImages.Where(x => x.ImageUrl == imgURL).FirstOrDefault();
                if(img != null)
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

        public async Task<List<TblImage>> GetImgUrlProductItem(Guid productItemId)
        {
            var query = from c in context.TblImages
                        where c.ProductItemId.Equals(productItemId)
                        select new { c };
            var result = await query.Select(x => new TblImage()
            {
                Id = x.c.Id,
                ProductId = x.c.ProductId,
                ImageUrl = x.c.ImageUrl
            }).ToListAsync();
            return result;
        }

        public async Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl)
        {
            var imgCategory = _context.TblImages.Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (imgCategory != null)
            {
                imgCategory.ImageUrl = imgUrl;
            }
            _context.Update(imgCategory);
            await _context.SaveChangesAsync();
            return imgCategory;
        }

        public async Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl)
        {
            var imgProduct = _context.TblImages.Where(x => x.ProductId == ProductID).FirstOrDefault();
            if (imgProduct != null)
            {
                imgProduct.ImageUrl = ImgUrl;
            }
            _context.Update(imgProduct);
            await _context.SaveChangesAsync();
            return imgProduct;
        }

        public async Task<bool> UpdateImgForProductItem(Guid ProductItemID, List<string> ImgUrls)
        {
            bool result = false;
            foreach (string imgUrl in ImgUrls) {
                var imgProduct = _context.TblImages.Where(x => x.ProductItemId == ProductItemID).FirstOrDefault();
                if (imgProduct != null)
                {
                    imgProduct.ImageUrl = imgUrl;
                }
                 _context.Update(imgProduct);
                await _context.SaveChangesAsync();
                result = true;
                if(result == false){ return result; }
            }
            return result;
        }
    }
}
