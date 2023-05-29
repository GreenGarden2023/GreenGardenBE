using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RevenueRepo
{
    public interface IRevenueRepo
    {
        Task<List<TblRentOrder>> getTotalRentOrderCompletedByDateRange(DateTime startDate, DateTime endDate);
        Task<List<TblSaleOrder>> getTotalSaleOrderCompletedByDateRange(DateTime startDate, DateTime endDate);
        Task<List<TblServiceOrder>> getTotalServiceOrderCompletedByDateRange(DateTime startDate, DateTime endDate);
        Task<List<TblTakecareComboOrder>> getTotalServiceComboOrderCompletedByDateRange(DateTime startDate, DateTime endDate);

    }
}
