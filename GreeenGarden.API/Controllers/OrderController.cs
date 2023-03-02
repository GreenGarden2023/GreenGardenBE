using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.OrderModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("order/")]
    [Authorize]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService service)
        {
            _service = service;
        }


        [HttpPost("create-order")]
        public async Task<IActionResult> createOrder([FromBody] OrderModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createOrder(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }


        //[HttpPost("add-addendum-by-order")]
        //public async Task<IActionResult> addAddendumByOrder([FromBody] addendumToAddByOrderModel model)
        //{
        //    string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
        //    var result = await _service.addAddendumByOrder(token, model);
        //    if (result.IsSuccess) return Ok(result);
        //    return BadRequest(result);
        //}

        [HttpGet("get-detail-addendum")]
        public async Task<IActionResult> getDetailAddendum(Guid addendumId)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getDetailAddendum(addendumId);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-list-addendum")]
        public async Task<IActionResult> getListAddendum(Guid orderId)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getListAddendum(token, orderId);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-list-order")]
        public async Task<IActionResult> getListOrder()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.getListOrder(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
