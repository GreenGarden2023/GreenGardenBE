using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Business.Service.ServiceServicer;
using GreeenGarden.Data.Models.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("service1/")]
    [Authorize]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly IServiceServicer _service;
        public ServiceController(IServiceServicer service)
        {
            _service = service;
        }

        [HttpGet("get-list-service-by-customer")]
        public async Task<IActionResult> getListServiceByCustomer()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getListServiceByCustomer(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("change-status")]
        public async Task<IActionResult> changeStatus(Guid serviceID, string status)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, serviceID, status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("create-service")]
        public async Task<IActionResult> createService( ServiceCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createService(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        
    }
}
