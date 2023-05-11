using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceDetailRepo
{
    public interface IServiceDetailRepo : IRepository<TblServiceDetail>
    {
        Task<List<ServiceDetailResModel>> GetServiceDetailByServiceID(Guid serviceID);
        Task<ServiceDetailResModel> GetServiceDetailByID(Guid serviceDetailID);
        Task<bool> UpdateServiceDetailManager(ServiceDetailUpdateModelManager serviceDetail);
        Task<bool> UpdateCareGuideByUserTree(UpdateCareGuideByTechnModel model);

    }
}

