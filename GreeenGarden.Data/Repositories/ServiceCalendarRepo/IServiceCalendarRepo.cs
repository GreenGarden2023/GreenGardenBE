using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceCalendarRepo
{
	public interface IServiceCalendarRepo : IRepository<TblServiceCalendar>
	{
		Task<bool> UpdateServiceCalendar(ServiceCalendarUpdateModel serviceCalendarUpdateModel);
	}
}

