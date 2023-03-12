using GreeenGarden.Data.Models.RequestModel;
using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.RequestService
{
    public interface IRequestService
    {
        Task<ResultModel> createRequest(string token, RequestCreateModel requestModel);
        Task<ResultModel> getListRequest(string token);
    }
}
