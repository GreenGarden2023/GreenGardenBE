using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<TblImage> GetImgUrl(Guid? imgId, Guid? categoryId, Guid? ProductItemID, Guid? FeedbackID, Guid? productId)
        {
            if (imgId != null) return await _context.TblImages.Where(x => x.Id == imgId).FirstAsync();
            if (categoryId != null) return await _context.TblImages.Where(x => x.CategoryId == categoryId).FirstAsync();
            if (ProductItemID != null) return await _context.TblImages.Where(x => x.ProductItemId == ProductItemID).FirstAsync();
            if (productId != null) return await _context.TblImages.Where(x => x.ProductId == productId).FirstAsync();
            if (FeedbackID != null) return await _context.TblImages.Where(x => x.FeedbackId == FeedbackID).FirstAsync();
            return null;
        }

        public async Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl)
        {
            var imgCategory = _context.TblImages.Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (imgCategory != null) {
                imgCategory.ImageUrl = imgUrl;
            }
            _context.Update(imgCategory);
            await _context.SaveChangesAsync();
            return imgCategory;
        }
    }
}
