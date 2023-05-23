using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RevenueRepo
{
    public class RevenueRepo : IRevenueRepo
    {
        public Task<List<TblRentOrder>> getTotalRentOrderCompletedByDateRange(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
