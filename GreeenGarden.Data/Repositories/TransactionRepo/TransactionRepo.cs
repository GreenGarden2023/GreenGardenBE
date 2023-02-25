using GreeenGarden.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.TransactionRepo
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly GreenGardenDbContext _context;
        public TransactionRepo(GreenGardenDbContext context) 
        {
            _context = context;
        }
        public async Task<TblTransaction> insert(TblTransaction transaction)
        {
            await _context.TblTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<TblPayment> insertPayment(TblPayment payment)
        {
            await _context.TblPayments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<TblAddendum> miniusAddendumtAmount(Guid addendumId, double? miniusMoney)
        {
            var addendum = await _context.TblAddendums.Where(x=>x.Id== addendumId).FirstOrDefaultAsync();
            addendum.RemainMoney -= miniusMoney;
            _context.TblAddendums.Update(addendum);
            await _context.SaveChangesAsync();
            return addendum;
        }

    }
}
