using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SubProductModel;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SubProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.SubProductService
{
    public class SubProductService : ISubProductService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ISubProductRepo _subProductRepo;
        public SubProductService(ISubProductRepo subProductRepo)
        {
            _subProductRepo = subProductRepo;
            _decodeToken = new DecodeToken();
        }
        public async Task<ResultModel> createProductSize(SizeItemRequestModel model, string token)
        {
            var result = new ResultModel();
            try
            {
                // Code
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
