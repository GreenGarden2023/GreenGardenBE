using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Repositories.GenericRepository;
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

        public TblSubProduct queryDetailBySubId(Guid? SubId)
        {
            var subProduct = _context.TblSubProducts.Where(x=>x.Id== SubId).FirstOrDefault();
            if (subProduct == null) return null;
            return subProduct;
        }

        public void updateSubProduct(TblSubProduct subProduct)
        {
            _context.TblSubProducts.Update(subProduct);
            _context.SaveChangesAsync();
        }
    }
}
