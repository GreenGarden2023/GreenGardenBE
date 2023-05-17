using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboServiceDetailRepo
{
	public interface ITakecareComboServiceDetailRepo : IRepository<TblTakecareComboServiceDetail>
    {
		Task<TakecareComboServiceDetail> GetTakecareComboServiceDetail(Guid takecareComboServiceID);
	}
}

