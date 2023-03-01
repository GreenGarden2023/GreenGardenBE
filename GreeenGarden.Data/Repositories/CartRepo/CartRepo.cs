using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
        
        public async Task<TblCart> GetCart(Guid UserID, bool? isForRent)
        {
            return await _context.TblCarts.Where(x => x.UserId.Equals(UserID) && x.IsForRent == isForRent).FirstOrDefaultAsync();
        }
        
        public async Task<TblSizeProductItem> GetSizeProductItem(Guid? SizeProductItemID)
        {
            return await _context.TblSizeProductItems.Where(x => x.Id.Equals(SizeProductItemID)).FirstOrDefaultAsync();
        }

        public async Task<TblCartDetail> AddProductItemToCart(TblCartDetail model)
        {
            await _context.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<List<TblCartDetail>> GetListCartDetail(Guid cartID)
        {
            return await _context.TblCartDetails.Where(x=>x.CartId.Equals(cartID)).ToListAsync();
        }

        public async Task<bool> RemoveCartDetail(TblCartDetail model)
        {
            _context.TblCartDetails.Remove(model);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
