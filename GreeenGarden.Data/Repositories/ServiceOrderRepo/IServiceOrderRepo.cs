using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
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
        //tblUser
        Task<TblUser> getTblUserByUsername(string username);
        Task<TblUser> getTblUserByID(Guid userID);
        Task<List<UserResModel>> getListTecnician();

        //tblImage
        Task<List<string>> getImgUrlByUserTreeID(Guid userTreeID);

        //tblUserTree
        Task<TblUserTree> getUserTreeByID(Guid userTreeID);

        //tblServiceUserTree
        Task<TblServiceUserTree> GetTblServiceUserTree(Guid serviceUserTreeID);
        Task<List<TblServiceUserTree>> GetListTblServiceUserTreeByServiceID(Guid serviceID);

        Task<bool> UpdateServiceUserTree(TblServiceUserTree entity);
        Task<bool> DeleteServiceUserTree(TblServiceUserTree entity);

        //tblService
        Task<TblService> GetTblServiceByID(Guid serviceID);

        //tblServiceOrder
        Task<bool> createServiceOrder(TblServiceOrder entity);
        Task<TblServiceOrder> GetTblServiceOrderByID(Guid serOderID);
        Task<bool> updateServiceOrder(TblServiceOrder entity);
        Task<ListServiceOrderResModel> GetListServiceOrder(TblUser user);
        Task<List<ServiceOrderResManagerModel>> GetListServiceOrderByManager();
        Task<DetailServiceOrderResModel> GetDetailServiceOrder(Guid SerOrderID);
        Task<List<DetailServiceOrderResModel>> GetListServiceOrderByTechnician(Guid techID);
        Task<bool> removeAll(Guid userID);
        Task<bool> changeStatusServiceOrder(ServiceOrderChangeStatusModel model);


    }
}
