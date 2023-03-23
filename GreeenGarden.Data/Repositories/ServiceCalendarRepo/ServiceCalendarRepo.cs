using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceCalendarRepo
{
	public class ServiceCalendarRepo : Repository<TblServiceCalendar>, IServiceCalendarRepo
	{
		private readonly GreenGardenDbContext _context;
		public ServiceCalendarRepo(GreenGardenDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<bool> UpdateServiceCalendar(ServiceCalendarUpdateModel serviceCalendarUpdateModel)
        {
			TblServiceCalendar tblServiceCalendar = await _context.TblServiceCalendars.Where(x => x.Id.Equals(serviceCalendarUpdateModel.ServiceCalendarId)).FirstOrDefaultAsync();
			if (tblServiceCalendar != null)
			{
				tblServiceCalendar.NextServiceDate = serviceCalendarUpdateModel.NextServiceDate;
				tblServiceCalendar.ReportFileUrl = serviceCalendarUpdateModel.ReportFileURL;
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

