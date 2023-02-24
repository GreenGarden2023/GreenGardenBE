using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService service)
        {
            _service = service;
        }


        [HttpPost("create-Order")]
        public async Task<IActionResult> createOrder([FromBody] OrderModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1]; 
            var result = await _service.createOrder(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
