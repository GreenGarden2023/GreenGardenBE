using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RevenueRepo
{
    public class RevenueRepo : IRevenueRepo
    {
        private readonly GreenGardenDbContext _context;
        public RevenueRepo( GreenGardenDbContext context) 
        {
            _context = context;
        }
        public async Task<List<TblRentOrder>> getTotalRentOrderCompletedByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.TblRentOrders.Where(x => x.Status.Equals(Status.COMPLETED) 
            && x.EndDateRent >= startDate && x.EndDateRent <= endDate).ToListAsync();
        }

        public async Task<List<TblSaleOrder>> getTotalSaleOrderCompletedByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.TblSaleOrders.Where(x => x.Status.Equals(Status.COMPLETED)
            && x.CreateDate >= startDate && x.CreateDate <= endDate).ToListAsync();
        }

        public async Task<List<TblTakecareComboOrder>> getTotalServiceComboOrderCompletedByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.TblTakecareComboOrders.Where(x => x.Status.Equals(Status.COMPLETED)
            && x.ServiceEndDate >= startDate && x.ServiceEndDate <= endDate).ToListAsync();
        }

        public async Task<List<TblServiceOrder>> getTotalServiceOrderCompletedByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.TblServiceOrders.Where(x => x.Status.Equals(Status.COMPLETED)
            && x.ServiceEndDate >= startDate && x.ServiceEndDate <= endDate).ToListAsync();
        }
    }
}
