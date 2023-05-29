using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Models.TakecareComboCalendarModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ComboServiceCalendarRepo
{
	public interface IComboServiceCalendarRepo : IRepository<TblComboServiceCalendar>
    {
        Task<bool> UpdateServiceCalendar(TakecareComboCalendarUpdateModel takecareComboCalendarUpdateModel);
        Task<List<ComboServiceCalendarUserGetModel>> GetServiceCalendarsByUser(Guid userID, DateTime startDate, DateTime endDate);
        Task<ComboServiceCalendarGetModel> GetServiceCalendarsByTechnician(Guid technicianID, DateTime date);
        Task<Page<TblTakecareComboService>> GetServiceByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID);
        Task<List<TblComboServiceCalendar>> GetServiceCalendarsByServiceOrder(Guid serviceOrderID);
        Task<List<ServiceCalendarTodayResModel>> GetComboCalendarsTodayByTechnician(DateTime date, Guid technicianId);
    }
}

