using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace GreeenGarden.Data.Repositories.ServiceCalendarRepo
{
    public class ServiceCalendarRepo : Repository<TblServiceCalendar>, IServiceCalendarRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IServiceOrderRepo _serviceOrderRepo;
        private readonly IImageRepo _imageRepo;
        public ServiceCalendarRepo(GreenGardenDbContext context, IServiceOrderRepo serviceOrderRepo, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _serviceOrderRepo = serviceOrderRepo;
            _imageRepo = imageRepo;
        }

        public async Task<List<TblServiceCalendar>?> GetServiceCalendarsByServiceOrder(Guid serviceOrderId)
        {
            List<TblServiceCalendar> tblServiceCalendars = await _context.TblServiceCalendars.Where(x => x.ServiceOrderId.Equals(serviceOrderId)).OrderByDescending(x=>x.ServiceDate).ToListAsync();
            return tblServiceCalendars ?? null;
        }

        public async Task<ServiceCalendarGetModel> GetServiceCalendarsByTechnician(Guid technicianID, DateTime date)
        {
            var query = from sc in context.TblServiceCalendars
                        join so in context.TblServiceOrders
                        on sc.ServiceOrderId equals so.Id
                        where so.TechnicianId.Equals(technicianID) && sc.ServiceDate.Equals(date)
                        select new { sc, so };
            List<ServiceCalendarResModel> listServiceCalendar = await query.Select(x => new ServiceCalendarResModel()
            {
                Id = x.sc.Id,
                ServiceOrderId = x.so.Id,
                ServiceDate = x.sc.ServiceDate,
                NextServiceDate = x.sc.NextServiceDate,
                Sumary = x.sc.Sumary,
                Status = x.sc.Status,
            }).OrderByDescending(x=>x.ServiceDate).ToListAsync();
            foreach (ServiceCalendarResModel serviceCalendar in listServiceCalendar)
            {
                serviceCalendar.Images = await _imageRepo.GetImgUrlServiceCalendar(serviceCalendar.Id);
            }
            ServiceCalendarGetModel serviceCalendarGetModel = new()
            {
                TechnicianId = technicianID,
                Date = date,
                CalendarQuantity = listServiceCalendar.Count(),
                CalendarList = listServiceCalendar
            };
            return serviceCalendarGetModel;
        }

        public async Task<Page<TblService>> GetServiceByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID)
        {
            return await _context.TblServices.Where(x => x.TechnicianId.Equals(technicianID)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
        }

        public async Task<List<ServiceCalendarUserGetModel>> GetServiceCalendarsByUser(Guid userID, DateTime startDate, DateTime endDate)
        {
            List<DateTime> datesInRange = new();
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                datesInRange.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            var query = from sc in context.TblServiceCalendars
                        join so in context.TblServiceOrders
                        on sc.ServiceOrderId equals so.Id
                        where so.UserId.Equals(userID) && sc.ServiceDate >= startDate && sc.ServiceDate <= endDate
                        select new { sc, so };
            List<ServiceCalendarUserResModel> listServiceCalendar = await query.Select(x => new ServiceCalendarUserResModel()
            {
                Id = x.sc.Id,
                ServiceOrderId = x.so.Id,
                TechnicianId = (Guid)x.so.TechnicianId,
                ServiceDate = x.sc.ServiceDate,
                NextServiceDate = x.sc.NextServiceDate,
                Sumary = x.sc.Sumary,
                Status = x.sc.Status,
            }).OrderByDescending(x=>x.ServiceDate).ToListAsync();
            foreach (ServiceCalendarUserResModel serviceCalendarUserResModel in listServiceCalendar)
            {
                serviceCalendarUserResModel.Images = await _imageRepo.GetImgUrlServiceCalendar(serviceCalendarUserResModel.Id);
            }
            List<ServiceCalendarUserGetModel> result = new();
            foreach (DateTime date in datesInRange)
            {
                List<ServiceCalendarUserResModel> listServiceCalendarByDate = new();
                foreach (ServiceCalendarUserResModel serviceCalendar in listServiceCalendar)
                {
                    if (serviceCalendar.ServiceDate == date)
                    {
                        listServiceCalendarByDate.Add(serviceCalendar);
                    }
                }
                ServiceCalendarUserGetModel serviceCalendarGetModel = new()
                {
                    Date = date,
                    CalendarQuantity = listServiceCalendarByDate.Count(),
                    CalendarList = listServiceCalendarByDate
                };
                result.Add(serviceCalendarGetModel);
            }
            return result;
        }

        public async Task<bool> UpdateServiceCalendar(ServiceCalendarUpdateModel serviceCalendarUpdateModel)
        {
            TblServiceCalendar tblServiceCalendar = await _context.TblServiceCalendars.Where(x => x.Id.Equals(serviceCalendarUpdateModel.ServiceCalendarId)).FirstOrDefaultAsync();
            if (tblServiceCalendar != null)
            {
                tblServiceCalendar.NextServiceDate = serviceCalendarUpdateModel.NextServiceDate;
                tblServiceCalendar.Sumary = serviceCalendarUpdateModel.Sumary;
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

