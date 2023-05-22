using System;
using GreeenGarden.Data.Models.ServiceCalendarModel;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using GreeenGarden.Business.Service.TakecareComboCalendarService;
using GreeenGarden.Data.Models.TakecareComboCalendarModel;

namespace GreeenGarden.API.Controllers
{
    [Route("takecare-service-calendar/")]
    [ApiController]
    public class TakecareComboCalendarController : Controller
    {
        private readonly ITakecareComboCalendarService _takecareComboCalendarService;
        public TakecareComboCalendarController(ITakecareComboCalendarService takecareComboCalendarService)
		{
            _takecareComboCalendarService = takecareComboCalendarService;
        }
        [HttpPost("create-service-calendar")]
        [Authorize(Roles = " Manager, Admin, Technician")]
        public async Task<IActionResult> CreateServiceCalendar(TakecareComboCalendarInsertModel serviceCalendarInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboCalendarService.CreateServiceCalendar(token, serviceCalendarInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-service-order")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByServiceOrder(Guid serviceOrderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboCalendarService.GetServiceCalendarsByServiceOrder(token, serviceOrderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-technician")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByTechnician([FromQuery] GetComboServiceCalendarsByTechnician getServiceCalendarsByTechnician)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboCalendarService.GetServiceCalendarsByTechnician(token, getServiceCalendarsByTechnician);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-calendar-by-user")]
        [Authorize(Roles = "Manager, Admin, Technician, Customer")]
        public async Task<IActionResult> GetServiceCalendarsByUser([FromQuery] GetComboServiceCalendarsByUser getServiceCalendarsByUser)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboCalendarService.GetServiceCalendarsByUser(token, getServiceCalendarsByUser);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

