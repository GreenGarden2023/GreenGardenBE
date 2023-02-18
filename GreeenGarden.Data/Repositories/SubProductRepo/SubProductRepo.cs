using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.SubProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.SubProductRepo
{
    public class SubProductRepo : Repository<TblSubProduct> , ISubProductRepo
    {
        private readonly GreenGardenDbContext _context;
        public SubProductRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public bool checkSizeUnique(Guid? SubId)
        {
            var checkResult = _context.TblSubProducts.Join(
                _context.TblSizes, 
                x=>x.SizeId, 
                s=>s.Id,(x,s)=> new
                {x.Id,s.Name}).Where(o=>o.Id == SubId).FirstOrDefault();

            if (checkResult.Name == Size.UNIQUE)
            {
                return true;
            }
            return false;
        }

        public void increaseQuantity(Guid subId, int plus)
        {
            var result = _context.TblSubProducts.Where(x=>x.Id== subId).FirstOrDefault();

            throw new NotImplementedException();
        }

        public TblSubProduct queryDetailBySubId(Guid? SubId)
        {
            var subProduct = _context.TblSubProducts.Where(x=>x.Id== SubId).FirstOrDefault();
            if (subProduct == null) return null;
            return subProduct;
        }

        public async Task<SubProductAndSize> querySubAndSize(Guid SubId)
        {
            var query = from su in _context.TblSubProducts
                        join si in _context.TblSizes
                        on su.SizeId equals si.Id
                        where su.Id.Equals(SubId)
                        select new { su, si };
            var result = await query.Select(x => new SubProductAndSize()
            {
                subProductId = SubId,
                name = x.su.Name,
                price = x.su.Price,
                size = x.si.Name,
                maxPrice =x.su.MaxPrice,
                minPrice = x.su.MinPrice
            }).FirstOrDefaultAsync();
            return result;
        }

        public void updateSubProduct(TblSubProduct subProduct)
        {
            _context.TblSubProducts.Update(subProduct);
            _context.SaveChangesAsync();
        }


    }
}
