using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SubProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SubProductRepo
{
    public class SubProductRepo : Repository<TblSubProduct>, ISubProductRepo
    {
        private readonly GreenGardenDbContext _context;
        public SubProductRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public bool checkSizeUnique(Guid? SubId)
        {
            /* var checkResult = _context.TblSubProducts.Join(
                 _context.TblSizes, 
                 x=>x.SizeId, 
                 s=>s.Id,(x,s)=> new
                 {x.Id,s.Name}).Where(o=>o.Id == SubId).FirstOrDefault();

             if (checkResult.Name == Size.UNIQUE)
             {
                 return true;
             }*/
            return false;
        }


        public async Task<TblSubProduct> queryDetailBySubId(Guid? SubId)
        {
            /*var subProduct = await _context.TblSubProducts.Where(x=>x.Id== SubId).FirstAsync();
            if (subProduct == null) return null;
            return subProduct;*/
            return null;
        }

        public async Task<SubProductAndSize> querySubAndSize(Guid SubId)
        {
            /*var query = from su in _context.TblSubProducts
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
            return result;*/
            return null;
        }


        public void updateSubProduct(TblSubProduct subProduct)
        {
            /*_context.TblSubProducts.Update(subProduct);
            _context.SaveChangesAsync();*/
        }

        public async void updateWhenCreateItemUnique(Guid subId, double price)
        {
            /*var result = await _context.TblSubProducts.Where(x => x.Id == subId).FirstAsync();
            result.Quantity = result.Quantity + 1;
            if (price != null)
            {
                if (result.MaxPrice < price) result.MaxPrice = price;
                if (result.MinPrice > price) result.MinPrice = price;
            }
            _context.Update(result);
            await _context.SaveChangesAsync();*/
        }

        public async void updateWhenUpdateItemSimilar(Guid guid)
        {
            /*var result = await _context.TblSubProducts.Where(x => x.Id == guid).FirstAsync();
            result.Quantity = result.Quantity + 1;
           
            _context.Update(result);
            await _context.SaveChangesAsync();*/
        }
    }
}
