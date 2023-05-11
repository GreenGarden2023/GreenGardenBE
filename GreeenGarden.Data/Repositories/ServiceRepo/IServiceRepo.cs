using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
    public interface IServiceRepo : IRepository<TblService>
    {
        Task<bool> ChangeServiceStatus(Guid serviceId, string status);
        Task<bool> AssignTechnician(ServiceAssignModelManager serviceAssignModelManager);
        Task<List<TblService>> GetRequestByUser(Guid userId);
        Task<List<TblService>> GetAllRequest();
        Task<bool> UpdateServiceUserInfo(ServiceUpdateModelManager serviceUpdateModelManager);
        Task<bool> CheckServiceCode(string serviceCode);
        Task<TblService> GetServiceByServiceCode(string serviceCode);
        Task<bool> UpdateService(TblService entity);
        Task<TblService> GetServiceByServiceCode(ServiceSearchByCodeModel model);
    }
}

