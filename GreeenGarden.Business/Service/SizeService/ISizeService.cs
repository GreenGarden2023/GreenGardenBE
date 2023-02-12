using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.SizeService
{
    public interface ISizeService
    {
        public Task<ResultModel> createSize(string sizeName, string token);
    }
}
