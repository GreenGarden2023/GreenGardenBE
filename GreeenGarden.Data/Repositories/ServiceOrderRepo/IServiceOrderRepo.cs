using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public interface IServiceOrderRepo : IRepository<TblServiceOrder>
    {
        public Task<TblUser> GetUser(string username);

        //---Create Order
        Task<bool> updateRequestDetail(TblRequestDetail entities);

        Task<List<TechicianModel>> getTechnician();
        Task<List<TblRequestDetail>> getListRequestDetail(Guid requestID);
        Task<bool> removeRequestDetail(TblRequestDetail entities);
        //---Get List Order
        Task<ServiceOrderResponseModel> getServiceOrder(Guid userID);

    }
}
