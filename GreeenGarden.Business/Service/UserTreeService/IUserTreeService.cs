using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.UserTreeService
{
    public interface IUserTreeService
    {
        Task<ResultModel> createUserTree(string token, UserTreeCreateModel model);
        Task<ResultModel> getListUserTree(string token);
        Task<ResultModel> getDetailUserTree(string token, Guid userTreeID);
        Task<ResultModel> updateUserTree(string token, UserTreeUpdateModel model);
        Task<ResultModel> changeStatus(string token, Guid userTreeID, string status);
    }
}
