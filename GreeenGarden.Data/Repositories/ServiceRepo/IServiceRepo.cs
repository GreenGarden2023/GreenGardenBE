using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
    public interface IServiceRepo : IRepository<TblService>
    {
        //tblUser
        Task<TblUser> getTblUserByUsername(string username);

        //tblImage
        Task<List<string>> getImgUrlByUserTreeID(Guid userTreeID);

        //tblServiceUserTree
        Task<bool> insertServiceUserTree(TblServiceUserTree entities);

        //tblUserTree
        Task<TblUserTree> getUserTreeByID(Guid userTreeID);

        //tblService
        Task<ListServiceByCustomerResModel> GetListServiceByCustomer(TblUser user);
        Task<List<ServiceByManagerResModel>> GetListServiceByManager();
        Task<DetailServiceByCustomerResModel> GetDetailServiceByCustomer(Guid serviceID);
        Task<TblService> GetTblService(Guid serviceID);
        Task<List<TblServiceUserTree>> GetListTblServiceUserTree(Guid serviceID);
        Task<TblServiceUserTree> GetTblServiceUserTree(Guid serviceID);
        Task<bool> UpdateService(TblService entities);
        Task<bool> UpdateServiceUserTree(TblServiceUserTree entities);
        Task<bool> DeleteServiceUserTree(TblServiceUserTree entities);
        Task<bool> ChangeStatusService(Guid serviceID, string status);
    }
}
