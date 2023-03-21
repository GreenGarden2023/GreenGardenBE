using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceDetailRepo
{
	public interface IServiceDetailRepo : IRepository<TblServiceDetail>
    {
		Task<List<ServiceDetailResModel>> GetServiceDetailByServiceID(Guid serviceID);
	}
}

