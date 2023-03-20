using System;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;

namespace GreeenGarden.Business.Service.UserTreeService
{
	public interface IUserTreeService
	{
        Task<ResultModel> CreateUserTree(string token, UserTreeInsertModel userTreeInsertModel);
    }
}

