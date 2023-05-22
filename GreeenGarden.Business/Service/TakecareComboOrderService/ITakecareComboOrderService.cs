using System;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboOrder;

namespace GreeenGarden.Business.Service.TakecareComboOrderService
{
	public interface ITakecareComboOrderService
	{
		Task<ResultModel> CreateTakecareComboOrder(TakecareComboOrderCreateModel takecareComboOrderCreateModel, string token);
		Task<ResultModel> GetTakecareComboOrderByID(Guid takecareComboOdderID, string token);
		Task<ResultModel> GetAllTakcareComboOrder(PaginationRequestModel pagingModel, string status, string token);
		Task<ResultModel> GetAllTakcareComboOrderForTechnician(PaginationRequestModel pagingModel, TakecareComboOrderTechnicianReqModel model, string token);
		Task<ResultModel> ChangeTakecareComboOrderStatus(Guid id, string status, string token);
        Task<ResultModel> CancelTakecareComboOrder(Guid id, string cancelReason, string token);
    }
}

