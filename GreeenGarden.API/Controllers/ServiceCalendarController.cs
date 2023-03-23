using System;
using System.Data;
using GreeenGarden.Business.Service.ServiceCalendarService;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("service-calendar/")]
    [ApiController]
    public class ServiceCalendarController : Controller
	{
		private readonly IServiceCalendarService _serviceCalendarService;
		public ServiceCalendarController(IServiceCalendarService serviceCalendarService)
        {
			_serviceCalendarService = serviceCalendarService;
		}
        [HttpPost("create-service-calendar")]
        [Authorize(Roles = "Staff, Manager, Admin, Technician")]
        public async Task<IActionResult> CreateServiceCalendar(ServiceCalendarInsertModel serviceCalendarInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.CreateServiceCalendar(token, serviceCalendarInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

