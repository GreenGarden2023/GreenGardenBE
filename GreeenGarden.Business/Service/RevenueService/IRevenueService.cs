using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.RevenueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.RevenueService
{
    public interface IRevenueService
    {
        Task<ResultModel> GetRevenueByDateRange(string token, RevenueReqByDateModel model);
    }
}
