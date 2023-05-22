using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboCalendarModel;

namespace GreeenGarden.Business.Service.TakecareComboCalendarService
{
	public interface ITakecareComboCalendarService
	{
        Task<ResultModel> CreateServiceCalendar(string token, TakecareComboCalendarInsertModel serviceCalendarInsertModel);
        Task<ResultModel> GetServiceCalendarsByUser(string token, GetComboServiceCalendarsByUser getServiceCalendarsByUser);
        Task<ResultModel> GetServiceCalendarsByTechnician(string token, GetComboServiceCalendarsByTechnician getServiceCalendarsByTechnician);
        Task<ResultModel> GetServiceCalendarsByServiceOrder(string token, Guid serviceOrderID);
    }
}

