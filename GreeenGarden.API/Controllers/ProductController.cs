using GreeenGarden.Business.Service.ProductService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("product/")]
    //[Authorize]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpGet("get-products-by-category")]
        public async Task<IActionResult> getAllProductByCategory([FromQuery] PaginationRequestModel pagingModel, Guid categoryId)
        {
            var result = await _service.getAllProductByCategory(pagingModel, categoryId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("create-product")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createProduct([FromForm] ProductCreateRequestModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createProduct(model, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

    }
}
