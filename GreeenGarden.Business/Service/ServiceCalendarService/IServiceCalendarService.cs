using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;

namespace GreeenGarden.Business.Service.ServiceCalendarService
{
    public interface IServiceCalendarService
    {
        Task<ResultModel> CreateServiceCalendar(string token, ServiceCalendarInsertModel serviceCalendarInsertModel);
        Task<ResultModel> GetServiceCalendarsByUser(string token, GetServiceCalendarsByUser getServiceCalendarsByUser);
        Task<ResultModel> GetServiceCalendarsByTechnician(string token, GetServiceCalendarsByTechnician getServiceCalendarsByTechnician);
        Task<ResultModel> GetServiceCalendarsTodayByTechnician(string token);
        Task<ResultModel> GetServiceCalendarsByServiceOrder(string token, Guid serviceOrderID);
    }
}

