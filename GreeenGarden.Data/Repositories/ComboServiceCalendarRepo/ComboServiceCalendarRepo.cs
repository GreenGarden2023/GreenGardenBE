using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Models.TakecareComboCalendarModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ComboServiceCalendarRepo
{
	public class ComboServiceCalendarRepo : Repository<TblComboServiceCalendar>, IComboServiceCalendarRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IImageRepo _imageRepo;

        public ComboServiceCalendarRepo(GreenGardenDbContext context, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _imageRepo = imageRepo;
        }

        public async Task<Page<TblTakecareComboService>> GetServiceByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID)
        {
            return await _context.TblTakecareComboServices.Where(x => x.TechnicianId.Equals(technicianID)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
        }

        public async Task<List<TblComboServiceCalendar>> GetServiceCalendarsByServiceOrder(Guid serviceOrderID)
        {
            List<TblComboServiceCalendar> tblServiceCalendars = await _context.TblComboServiceCalendars.Where(x => x.TakecareComboOrderId.Equals(serviceOrderID)).OrderByDescending(x => x.ServiceDate).ToListAsync();
            return tblServiceCalendars ?? null;
        }

        public async Task<ComboServiceCalendarGetModel> GetServiceCalendarsByTechnician(Guid technicianID, DateTime date)
        {
            var query = from sc in context.TblComboServiceCalendars
                        join so in context.TblTakecareComboOrders
                        on sc.TakecareComboOrderId equals so.Id
                        where so.TechnicianId.Equals(technicianID) && sc.ServiceDate.Equals(date)
                        select new { sc, so };
            List<ComboServiceCalendarResModel> listServiceCalendar = await query.Select(x => new ComboServiceCalendarResModel()
            {
                Id = x.sc.Id,
                ServiceOrderId = x.so.Id,
                ServiceDate = x.sc.ServiceDate,
                NextServiceDate = x.sc.NextServiceDate,
                Sumary = x.sc.Sumary,
                Status = x.sc.Status,
            }).OrderByDescending(x => x.ServiceDate).ToListAsync();

            foreach (ComboServiceCalendarResModel serviceCalendar in listServiceCalendar)
            {
                serviceCalendar.Images = await _imageRepo.GetImgUrlComboServiceCalendar(serviceCalendar.Id);
            }
            ComboServiceCalendarGetModel serviceCalendarGetModel = new()
            {
                TechnicianId = technicianID,
                Date = date,
                CalendarQuantity = listServiceCalendar.Count(),
                CalendarList = listServiceCalendar
            };
            return serviceCalendarGetModel;
        }

        public async Task<List<ComboServiceCalendarUserGetModel>> GetServiceCalendarsByUser(Guid userID, DateTime startDate, DateTime endDate)
        {
            List<DateTime> datesInRange = new();
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                datesInRange.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            var query = from sc in context.TblComboServiceCalendars
                        join so in context.TblTakecareComboOrders
                        on sc.TakecareComboOrderId equals so.Id
                        where so.UserId.Equals(userID) && sc.ServiceDate >= startDate && sc.ServiceDate <= endDate
                        select new { sc, so };
            List<ComboServiceCalendarUserResModel> listServiceCalendar = await query.Select(x => new ComboServiceCalendarUserResModel()
            {
                Id = x.sc.Id,
                ServiceOrderId = x.so.Id,
                TechnicianId = (Guid)x.so.TechnicianId,
                ServiceDate = x.sc.ServiceDate,
                NextServiceDate = x.sc.NextServiceDate,
                Sumary = x.sc.Sumary,
                Status = x.sc.Status,
            }).OrderByDescending(x => x.ServiceDate).ToListAsync();
            foreach (ComboServiceCalendarUserResModel serviceCalendarUserResModel in listServiceCalendar)
            {
                serviceCalendarUserResModel.Images = await _imageRepo.GetImgUrlComboServiceCalendar(serviceCalendarUserResModel.Id);
            }
            List<ComboServiceCalendarUserGetModel> result = new();
            foreach (DateTime date in datesInRange)
            {
                List<ComboServiceCalendarUserResModel> listServiceCalendarByDate = new();
                foreach (ComboServiceCalendarUserResModel serviceCalendar in listServiceCalendar)
                {
                    if (serviceCalendar.ServiceDate == date)
                    {
                        listServiceCalendarByDate.Add(serviceCalendar);
                    }
                }
                ComboServiceCalendarUserGetModel serviceCalendarGetModel = new()
                {
                    Date = date,
                    CalendarQuantity = listServiceCalendarByDate.Count(),
                    CalendarList = listServiceCalendarByDate
                };
                result.Add(serviceCalendarGetModel);
            }
            return result;
        }

        public async Task<bool> UpdateServiceCalendar(TakecareComboCalendarUpdateModel takecareComboCalendarUpdateModel)
        {
            TblComboServiceCalendar tblServiceCalendar = await _context.TblComboServiceCalendars.Where(x => x.Id.Equals(takecareComboCalendarUpdateModel.ServiceCalendarId)).FirstOrDefaultAsync();
            if (tblServiceCalendar != null)
            {
                tblServiceCalendar.NextServiceDate = DateTime.ParseExact(takecareComboCalendarUpdateModel.NextServiceDate, "dd/MM/yyyy", null);
                tblServiceCalendar.Sumary = takecareComboCalendarUpdateModel.Sumary;
                tblServiceCalendar.Status = ServiceCalendarStatus.DONE;
                _ = _context.Update(tblServiceCalendar);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

