using GreeenGarden.Business.Service.ServiceCalendarService;
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
        [Authorize(Roles = " Manager, Admin, Technician")]
        public async Task<IActionResult> CreateServiceCalendar(ServiceCalendarInsertModel serviceCalendarInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.CreateServiceCalendar(token, serviceCalendarInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-service-order")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByServiceOrder(Guid serviceOrderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.GetServiceCalendarsByServiceOrder(token, serviceOrderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-technician")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByTechnician([FromQuery] GetServiceCalendarsByTechnician getServiceCalendarsByTechnician)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.GetServiceCalendarsByTechnician(token, getServiceCalendarsByTechnician);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendars-today-by-technician")]
        public async Task<IActionResult> GetServiceCalendarsTodayByTechnician()
        {
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.GetServiceCalendarsTodayByTechnician();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-user")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByUser([FromQuery] GetServiceCalendarsByUser getServiceCalendarsByUser)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _serviceCalendarService.GetServiceCalendarsByUser(token, getServiceCalendarsByUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

