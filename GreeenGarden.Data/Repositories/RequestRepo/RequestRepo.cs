using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RequestRepo
{
    public class RequestRepo : Repository<TblRequest>, IRequestRepo
    {

        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public RequestRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public Task<TblRequest> Create(TblRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
