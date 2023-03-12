using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.CartRepo
{
    public class CartRepo : Repository<TblCart>, ICartRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public CartRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<TblUser> GetByUserName(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<TblCart> GetCart(Guid UserID)
        {
            return await _context.TblCarts.Where(x => x.UserId.Equals(UserID)).FirstOrDefaultAsync();
        }

        public async Task<productItemDetail> GetProductItemDetail(Guid? SizeProductItemID)
        {
            var proItemDetail = await _context.TblProductItemDetails.Where(x => x.Id.Equals(SizeProductItemID)).FirstOrDefaultAsync();
            var size = await _context.TblSizes.Where(x=>x.Id.Equals(proItemDetail.SizeId)).FirstOrDefaultAsync();
            var sizeTemp = new size()
            {
                id = size.Id,
                sizeName = size.Name,
            };
            return new productItemDetail()
            {
                Id = proItemDetail.Id,
                Content = null,
                SizeId = proItemDetail.SizeId,
                ProductItemId = proItemDetail.ProductItemId,
                RentPrice = proItemDetail.RentPrice,
                SalePrice = proItemDetail.SalePrice,
                Quantity = proItemDetail.Quantity,
                Status = proItemDetail.Status,
                size = sizeTemp,
                imgUrl = await GetListImgBySizeProItem(proItemDetail.Id)
            };
        }

        public async Task<TblCartDetail> AddProductItemToCart(TblCartDetail model)
        {
            await _context.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<List<TblCartDetail>> GetListCartDetail(Guid cartID)
        {
            return await _context.TblCartDetails.Where(x => x.CartId.Equals(cartID)).ToListAsync();
        }

        public async Task<bool> RemoveCartDetail(TblCartDetail model)
        {
            _context.TblCartDetails.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TblProductItem> GetProductItem(Guid productItemID)
        {
            return await _context.TblProductItems.Where(x => x.Id.Equals(productItemID)).FirstOrDefaultAsync();
        }

        public async Task<TblSize> GetSize(Guid sizeID)
        {
            return await _context.TblSizes.Where(x => x.Id.Equals(sizeID)).FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetListImgBySizeProItem(Guid proItemDetailID)
        {
            var img = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(proItemDetailID)).ToListAsync();
            var result = new List<string>();
            foreach (var imgItem in img)
            {
                result.Add(imgItem.ImageUrl);
            }
            return result;
        }

        public async Task<TblProductItemDetail> GetProductItemDetails(Guid proDetailID)
        {
            return await _context.TblProductItemDetails.Where(x => x.Id.Equals(proDetailID)).FirstOrDefaultAsync();
        }
    }
}
