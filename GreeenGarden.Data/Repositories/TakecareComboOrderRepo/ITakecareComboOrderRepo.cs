using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboOrder;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboOrderRepo
{
	public interface ITakecareComboOrderRepo : IRepository<TblTakecareComboOrder>
    {
		Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrder(PaginationRequestModel paginationRequestModel, string status);
		Task<Page<TblTakecareComboOrder>> GetTakecreComboOrderByPhone(PaginationRequestModel paginationRequestModel, string status, string phone);
		Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrderForCustomer(PaginationRequestModel paginationRequestModel, string status, Guid userID);
		Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrderForTech(PaginationRequestModel paginationRequestModel, TakecareComboOrderTechnicianReqModel model);
        Task<bool> ChangeTakecareComboOrderStatus(Guid id, string status);
        Task<ResultModel> UpdateOrderDeposit(Guid orderID);
        Task<TblTakecareComboOrder> GetTakecareComboOrderByCode(string orderCode);
        Task<ResultModel> UpdateOrderRemain(Guid orderID, double payAmount);
        Task<bool> CancelOrder(Guid orderID, string cancelReason, Guid cancelBy);
    }
}

