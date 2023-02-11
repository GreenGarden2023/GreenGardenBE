using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public class ImageRepo : Repository<TblImage>, IImageRepo
    {
        public ImageRepo(GreenGardenDbContext context) : base(context)
        {
        }
    }
}
