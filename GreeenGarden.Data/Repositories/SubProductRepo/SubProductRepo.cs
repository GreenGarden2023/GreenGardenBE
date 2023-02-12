using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
