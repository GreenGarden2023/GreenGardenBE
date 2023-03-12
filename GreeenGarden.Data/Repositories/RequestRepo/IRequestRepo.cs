using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.RequestModel;
using GreeenGarden.Data.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RequestRepo
{
    public interface IRequestRepo : IRepository<TblRequest>
    {
        Task<TblUser> GetUserByUsername(string username);
        Task<bool> InsertRequestDetail(TblRequestDetail entities);
        Task<bool> InsertImage(TblImage entities);
        Task<RequestResponseModel> GetListRequest(Guid UserID);
        Task<bool> changeStatus(RequestUpdateStatusModel model);
    }
}
