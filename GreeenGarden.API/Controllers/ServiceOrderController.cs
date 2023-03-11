using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.ServiceOrderService;
using GreeenGarden.Data.Models.ServiceOrderModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("category/")]
    [Authorize]
    [ApiController]
    public class ServiceOrderController : Controller
    {
        private readonly IServiceOrderService _service;
        public ServiceOrderController(IServiceOrderService service)
        {
            _service = service;
        }



        [HttpGet("show-technician")]
        public async Task<IActionResult> showTechnician()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.showTechnician(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("show-list-serviceOrder")]
        public async Task<IActionResult> showListServiceOrder()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.showListServiceOrder(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("create-service-order")]
        public async Task<IActionResult> createServiceOrder(ServiceOrderCreateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createServiceOrder(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
