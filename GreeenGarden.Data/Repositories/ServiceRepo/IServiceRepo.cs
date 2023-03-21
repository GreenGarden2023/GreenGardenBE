using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
	public interface IServiceRepo : IRepository<TblService>
	{
		Task<bool> ChangeServiceStatus(Guid serviceId, string status);
		Task<List<TblService>> GetRequestByUser(Guid userId);
        Task<List<TblService>> GetAllRequest();
    }
}

