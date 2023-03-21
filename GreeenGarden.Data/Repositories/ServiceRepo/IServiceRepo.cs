using System;
using GreeenGarden.Data.Entities;
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
    }
}

