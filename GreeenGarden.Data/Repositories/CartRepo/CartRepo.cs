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

        public async Task<sizeProductItem> GetSizeProductItem(Guid? SizeProductItemID)
        {
            var sizeProItem = await _context.TblSizeProductItems.Where(x => x.Id.Equals(SizeProductItemID)).FirstOrDefaultAsync();
            var size = await _context.TblSizes.Where(x=>x.Id.Equals(sizeProItem.SizeId)).FirstOrDefaultAsync();
            return new sizeProductItem()
            {
                Id = sizeProItem.Id,
                Content= sizeProItem.Content,
                SizeId= sizeProItem.SizeId,
                ProductItemId= sizeProItem.ProductItemId,
                RentPrice= sizeProItem.RentPrice,
                SalePrice= sizeProItem.SalePrice,
                Quantity= sizeProItem.Quantity,
                Status= sizeProItem.Status,
                size = size
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
    }
}
