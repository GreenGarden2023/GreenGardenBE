using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.RevenueModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RevenueRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.RevenueService
{
    public class RevenueService : IRevenueService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRevenueRepo _revenueRepo;

        public RevenueService(IRevenueRepo revenueRepo)
        {
            _decodeToken = new DecodeToken();
            _revenueRepo = revenueRepo;
        }
        public async Task<ResultModel> GetRevenueByDateRange(string token, RevenueReqByDateModel model)
        {
            var result = new ResultModel();
            try
            {
                DateTime fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                DateTime toDate = ConvertUtil.convertStringToDateTime(model.toDate);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
