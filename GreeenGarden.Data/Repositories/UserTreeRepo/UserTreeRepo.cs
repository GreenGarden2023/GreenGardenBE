using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.UserTreeRepo
{
    public class UserTreeRepo : Repository<TblUserTree>, IUserTreeRepo
    {
        private readonly GreenGardenDbContext _context;
        public UserTreeRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
