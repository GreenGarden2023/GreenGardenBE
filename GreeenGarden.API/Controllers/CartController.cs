using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Data.Models.CartModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("cart/")]
    [Authorize]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ICartService _service;
        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet("get-cart")]
        public async Task<IActionResult> GetCart()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.GetCart(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.AddToCart(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("clean-cart")]
        public async Task<IActionResult> CleanCart()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.CleanCart(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
