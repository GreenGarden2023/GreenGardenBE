using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.TransactionRepo
{
    public class TransactionRepo : Repository<TblTransaction>, ITransactionRepo
    {
        private readonly GreenGardenDbContext _context;
        public TransactionRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }



    }
}
