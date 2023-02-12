using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.SizeRepo
{
    public class SizeRepo : Repository<TblSize> , ISizeRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public SizeRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
    }
}
