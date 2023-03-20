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
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.GetCart(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.AddToCart(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("clean-cart")]
        public async Task<IActionResult> CleanCart()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.CleanCart(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
