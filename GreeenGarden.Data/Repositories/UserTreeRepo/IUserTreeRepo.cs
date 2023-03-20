using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.UserTreeRepo
{
    public interface IUserTreeRepo : IRepository<TblUserTree>
    {
        Task<List<TblUserTree>> GetUserTrees(Guid userID);
        Task<bool> UpdateUserTree(UserTreeUpdateModel userTreeUpdateModel);
        Task<bool> ChangeUserTreeStatus(UserTreeStatusModel userTreeStatusModel);
    }
}
