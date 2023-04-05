using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Repositories.GenericRepository;
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

        public async Task<productItemDetail> GetProductItemDetail(Guid? productItemDetailID)
        {
            TblProductItemDetail? proItemDetail = await _context.TblProductItemDetails.Where(x => x.Id.Equals(productItemDetailID)).FirstOrDefaultAsync();
            TblSize? size = await _context.TblSizes.Where(x => x.Id.Equals(proItemDetail.SizeId)).FirstOrDefaultAsync();
            size sizeTemp = new()
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
                TransportFee = proItemDetail.TransportFee,
                Status = proItemDetail.Status,

                size = sizeTemp,
                imgUrl = await GetListImgBySizeProItem(proItemDetail.Id)
            };
        }

        public async Task<TblCartDetail> AddProductItemToCart(TblCartDetail model)
        {
            _ = await _context.AddAsync(model);
            _ = await _context.SaveChangesAsync();
            return model;
        }

        public async Task<List<TblCartDetail>> GetListCartDetail(Guid cartID)
        {
            return await _context.TblCartDetails.Where(x => x.CartId.Equals(cartID)).ToListAsync();
        }

        public async Task<bool> RemoveCartDetail(TblCartDetail model)
        {
            _ = _context.TblCartDetails.Remove(model);
            _ = await _context.SaveChangesAsync();
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
            List<TblImage> img = await _context.TblImages.Where(x => x.ProductItemDetailId.Equals(proItemDetailID)).ToListAsync();
            List<string> result = new();
            foreach (TblImage? imgItem in img)
            {
                result.Add(imgItem.ImageUrl);
            }
            return result;
        }

        public async Task<TblProductItemDetail> GetProductItemDetails(Guid proDetailID)
        {
            return await _context.TblProductItemDetails.Where(x => x.Id.Equals(proDetailID)).FirstOrDefaultAsync();
        }

        public async Task<List<TblCartDetail>> GetListCartDetailByType(Guid cartID, bool isForRent)
        {
            return await _context.TblCartDetails.Where(x => x.CartId.Equals(cartID) && x.IsForRent == isForRent).ToListAsync();
        }
    }
}
