using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public class OrderRepo : Repository<TblOrder>, IOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public OrderRepo( GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<TblProductItem> getProductToCompare(Guid productId)
        {
            return await _context.TblProductItems.Where(x => x.Id == productId).FirstAsync();
        }

        public async Task<TblUser> getUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName == username).FirstAsync();
        }

        public async Task<TblAddendum> insertAddendum(TblAddendum entities)
        {
            await _context.TblAddendums.AddAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TblAddendumProductItem> insertAddendumProductItem(TblAddendumProductItem entities)
        {
            await _context.TblAddendumProductItems.AddAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TblProductItem> minusQuantityProductItem(Guid productItemId, int quantity)
        {
            var productItem = await _context.TblProductItems.Where(x => x.Id == productItemId).FirstAsync();
            productItem.Quantity = productItem.Quantity - quantity;
            _context.Update(productItem);
            await _context.SaveChangesAsync();
            return productItem;
        }
    }
}
