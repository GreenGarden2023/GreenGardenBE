using System;
using GreeenGarden.Business.Service.TakecareComboService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboServiceModel;

namespace GreeenGarden.Business.Service.TakecareComboServiceServ
{
	public interface ITakecareComboServiceServ
	{
		Task<ResultModel> CreateTakecareComboService(TakecareComboServiceInsertModel takecareComboServiceInsertModel, string token);
        Task<ResultModel> GetTakecareComboServiceByID(Guid takecareComboServiceId, string token);
        Task<ResultModel> GetTakecareComboServiceByCode(string code, string token);
        Task<ResultModel> GetAllTakecareComboService(string status, string token);
        Task<ResultModel> GetAllTakecareComboServiceForTechnician(string status, string token, Guid technician);
        Task<ResultModel> ChangeTakecareComboServiceStatus(TakecareComboServiceChangeStatusModel takecareComboServiceChangeStatusModel, string token);
        Task<ResultModel> AssignTechnicianTakecareComboService(TakecareComboServiceAssignTechModel takecareComboServiceAssignTechModel, string token);
        Task<ResultModel> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel, string token);
        Task<ResultModel> CancelService(TakecareComboServiceCancelModel takecareComboServiceCancelModel, string token);
        Task<ResultModel> RejectService(TakecareComboServiceRejectModel takecareComboServiceRejectModel, string token);

    }
}

