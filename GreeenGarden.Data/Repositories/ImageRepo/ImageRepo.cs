using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public class ImageRepo : Repository<TblImage>, IImageRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ImageRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        //public async Task<TblImage> GetImgUrl(Guid? imgId, Guid? categoryId, Guid? ProductItemID, Guid? FeedbackID, Guid? productId)
        //{
        //    if (imgId != null) return await _context.TblImages.Where(x => x.Id == imgId).FirstAsync();
        //    if (categoryId != null) return await _context.TblImages.Where(x => x.CategoryId == categoryId).FirstAsync();
        //    if (ProductItemID != null) return await _context.TblImages.Where(x => x.ProductItemId == ProductItemID).FirstAsync();
        //    if (productId != null) return await _context.TblImages.Where(x => x.ProductId == productId).FirstAsync();
        //    if (FeedbackID != null) return await _context.TblImages.Where(x => x.FeedbackId == FeedbackID).FirstAsync();
        //    return null;
        //}

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
    }
}
