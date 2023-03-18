using GreeenGarden.Business.Service.ServiceOrderService;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("serviceOrder/")]
    [Authorize]
    [ApiController]
    public class ServiceOrderController : Controller
    {
        private readonly IServiceOrderService _service;
        public ServiceOrderController(IServiceOrderService service)
        {
            _service = service;
        }

        [HttpPost("create-service-order")]
        public async Task<IActionResult> createServiceOrder(ServiceOrderCreateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createServiceOrder(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("update-service-order")]
        public async Task<IActionResult> updateServiceOrder(ServiceUserUpdateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.updateServiceOrder(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-list-service-order-by-customer")]
        public async Task<IActionResult> getListServiceOrderByCustomer()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getListServiceOrderByCustomer(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-list-technician")]
        public async Task<IActionResult> getTechnician()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getTechnician(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("remove-all-by-customer")]
        public async Task<IActionResult> cleanServiceOrder()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.cleanServiceOrder(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-list-service-order-by-manager")]
        public async Task<IActionResult> getListServiceOrderByManager()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getListServiceOrderByManager(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-detail-service-order")]
        public async Task<IActionResult> getDetailServiceOrder(Guid SerOrderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getDetailServiceOrder(token, SerOrderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-list-service-order-by-technician")]
        public async Task<IActionResult> getListServiceOrderByTechnician()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getListServiceOrderByTechnician(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPatch("change-status")]
        public async Task<IActionResult> changeStatus(ServiceOrderChangeStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
