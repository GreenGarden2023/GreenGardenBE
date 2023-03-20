using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;

namespace GreeenGarden.Business.Service.UserTreeService
{
    public interface IUserTreeService
    {
        Task<ResultModel> CreateUserTree(string token, UserTreeInsertModel userTreeInsertModel);
        Task<ResultModel> GetUserTrees(string token);
        Task<ResultModel> UpdateUserTree(string token, UserTreeUpdateModel userTreeUpdateModel);
        Task<ResultModel> UpdateUserTreeStatus(string token, UserTreeStatusModel userTreeStatusModel);
    }
}

