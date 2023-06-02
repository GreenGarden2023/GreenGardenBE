using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
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
		Task<Page<TblTakecareComboService>> GetTakecareComboServiceByPhone(GetServiceByPhoneModel model, PaginationRequestModel pagingModel);
		Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByTech(string serviceCode, string status, Guid technician);
		Task<bool> ChangeTakecareComboServiceStatus(Guid takecareComboServiceID, string status);
		Task<bool> UpdateServiceOrderCareGuide(Guid takecareComboServiceOrderID, string careGuideUrl);
		Task<bool> AssignTechnicianTakecareComboService(Guid takecareComboServiceID, Guid technicianID);
		Task<bool> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel);
        Task<bool> CancelService(Guid takecareComboServiceID, string cancelReason, Guid cancelBy);
        Task<Page<TblTakecareComboOrder>> GetAllServiceOrderByRangDate(PaginationRequestModel pagingModel, DateTime fromDate, DateTime toDate);
        Task<bool> RejectService(Guid takecareComboServiceID, string reason, Guid userId);
    }
}

