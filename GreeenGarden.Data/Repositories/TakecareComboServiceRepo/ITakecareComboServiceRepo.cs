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
		Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByCustomer(string status, Guid userId);
		Task<TblTakecareComboService> GetTakecareComboServiceByCode(string code);
		Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByTech(string status, Guid technician);
		Task<bool> ChangeTakecareComboServiceStatus(Guid takecareComboServiceID, string status);
		Task<bool> UpdateServiceOrderCareGuide(Guid takecareComboServiceOrderID, string careGuideUrl);
		Task<bool> AssignTechnicianTakecareComboService(Guid takecareComboServiceID, Guid technicianID);
		Task<bool> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel);
        Task<bool> CancelService(Guid takecareComboServiceID, string cancelReason, Guid cancelBy);
        Task<bool> RejectService(Guid takecareComboServiceID, string reason, Guid userId);
    }
}

