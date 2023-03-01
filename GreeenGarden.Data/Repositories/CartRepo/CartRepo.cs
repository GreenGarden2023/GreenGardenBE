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

        public async Task<TblCartDetail> AddProductItemToCart(TblCartDetail model)
        {
            await _context.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<TblUser> GetByUserName(string username)
        {
            return await _context.TblUsers.Where(x=>x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<TblCart> GetCart(Guid UserID)
        {
            return await _context.TblCarts.Where(x => x.UserId.Equals(UserID)).FirstOrDefaultAsync();
        }

        public async Task<List<TblCartDetail>> GetCartDetail(Guid CartID)
        {
            return await _context.TblCartDetails.Where(x => x.CartId.Equals(CartID)).ToListAsync();
        }

        public async Task<CartShowModel> GetCartShow(Guid UserID)
        {
            var cart = await _context.TblCarts.Where(x=>x.UserId.Equals(UserID)).FirstOrDefaultAsync();
            if (cart == null) return null;
            var cartDetail = await _context.TblCartDetails.Where(x=>x.CartId.Equals(cart.Id)).ToListAsync();
            var result = new CartShowModel();
            result.Status = cart.Status;
            //result.TotalPrice = cart.TotalPrice;
            //result.Quantity= cart.Quantity;
            result.IsForRent = cart.IsForRent;
            result.Items = new List<Items>();

            foreach (var item in cartDetail)
            {
                var listItems = new Items();
                //listItems.ProductItemId = item.ProductItemId;
                //listItems.Quantity = item.Quantity;
                //listItems.Price = item.Price;
                result.Items.Add(listItems);
            }
            return result;
        }

        public async Task<TblProductItem> GetProductItem(Guid? ProductItemID)
        {
            return await _context.TblProductItems.Where(x => x.Id.Equals(ProductItemID)).FirstOrDefaultAsync();
        }

        public async Task<TblCart> UpdateCart(TblCart model)
        {
             _context.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }
    }
}
