using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboServiceRepo
{
	public interface ITakecareComboServiceRepo : IRepository<TblTakecareComboService>
    {
		Task<bool> CheckCodeDup(string code);
		Task<List<TblTakecareComboService>> GetAllTakecareComboService(string status);
		Task<TblTakecareComboService> GetTakecareComboServiceByCode(string code);
		Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByTech(string status, Guid technician);
		Task<bool> ChangeTakecareComboServiceStatus(Guid takecareComboServiceID, string status);
		Task<bool> AssignTechnicianTakecareComboService(Guid takecareComboServiceID, Guid technicianID);
		Task<bool> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel);
        Task<bool> CancelService(Guid takecareComboServiceID, string cancelReason, Guid cancelBy);
    }
}

