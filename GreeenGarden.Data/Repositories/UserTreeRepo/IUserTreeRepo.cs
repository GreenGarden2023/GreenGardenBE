using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.UserTreeRepo
{
    public interface IUserTreeRepo : IRepository<TblUserTree>
    {
        Task<List<TblUserTree>> GetUserTrees(Guid userID);
        Task<bool> UpdateUserTree(UserTreeUpdateModel userTreeUpdateModel);
        Task<bool> ChangeUserTreeStatus(UserTreeStatusModel userTreeStatusModel);
    }
}
