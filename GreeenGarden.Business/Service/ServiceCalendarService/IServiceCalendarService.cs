using System;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;

namespace GreeenGarden.Business.Service.ServiceCalendarService
{
	public interface IServiceCalendarService
	{
		Task<ResultModel> CreateServiceCalendar(string token, ServiceCalendarInsertModel serviceCalendarInsertModel);
	}
}

