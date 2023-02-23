using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SizeRepo
{
    public class SizeRepo : Repository<TblSize>, ISizeRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public SizeRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<TblSize>> GetProductItemSizes()
        {
            return await _context.TblSizes.ToListAsync();
        }
    }
}
