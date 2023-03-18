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
        //User
        Task<TblUser> GetTblUserByUsername(string roleID);
        Task<TblRole> GetTblRoleByUserID(Guid userID);

        //Img
        Task<bool> InsertImg(TblImage entities);
        Task<List<string>> getImgUrlByUTID(Guid UTID);
        Task<bool> updateImgUrlByUTID(Guid utID, List<string> urlImg);

        //UserTree
        Task<ListUserTreeResModel> GetListUserTreeByCustomer(TblUser user);
        Task<DetailUserTreeResModel> GetDetailUserTreeByCustomer(Guid utID);
        Task<bool> UpdateUserTreeByCustomer(UserTreeUpdateModel model);
        Task<bool> ChangeUserTreeByCustomer(Guid userTreeID, string status);
    }
}
