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
        Task<ResultModel> CreateRequest(string token, RequestCreateModel model);
        Task<ResultModel> GetListRequest(string token);
        Task<ResultModel> ChangeStatus(string token, RequestUpdateStatusModel model);
    }
}
