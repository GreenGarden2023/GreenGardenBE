using System.Collections.Generic;
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

        public async Task<List<TblTransaction>> GetTransactionByDateRange(DateTime rangeStart, DateTime rangeEnd)
        {
            List<TblTransaction> tblTransactions = new List<TblTransaction>();
            tblTransactions = await _context.TblTransactions.Where(x => x.DatetimePaid >= rangeStart && x.DatetimePaid <= rangeEnd).ToListAsync();
            return tblTransactions;
        }

        public async Task<List<TblTransaction>> GetTransactionByOrder(Guid orderId, string orderType)
        {
            List<TblTransaction> tblTransactions = new List<TblTransaction>();
            if (orderType.Trim().ToLower().Equals("rent")) {
                tblTransactions = await _context.TblTransactions.Where(x => x.RentOrderId.Equals(orderId)).ToListAsync();
                return tblTransactions;
            }else if (orderType.Trim().ToLower().Equals("sale"))
            {
                tblTransactions = await _context.TblTransactions.Where(x => x.SaleOrderId.Equals(orderId)).ToListAsync();
                return tblTransactions;
            }
            else if (orderType.Trim().ToLower().Equals("service"))
            {
                tblTransactions = await _context.TblTransactions.Where(x => x.ServiceOrderId.Equals(orderId)).ToListAsync();
                return tblTransactions;
            }
            else
            {
                return null;
            }
        }
    }
}
