using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SubProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.SubProductService
{
    public interface ISubProductService
    {
        public Task<ResultModel> createProductSize(SizeItemRequestModel model, string token);
    }
}
