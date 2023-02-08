using GreeenGarden.Business.Service.OrderService;
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


        [HttpGet("CheckWholesaleProduct")]
        public async Task<IActionResult> checkWholesaleProduct(Guid subProductId, int quantity)
        {
            var result = await _service.checkWholesaleProduct(subProductId, quantity);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



        [HttpGet("CheckRetailProduct")]
        public async Task<IActionResult> checkRetailProduct(Guid productItemId)
        {
            var result = await _service.checkRetailProduct(productItemId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
