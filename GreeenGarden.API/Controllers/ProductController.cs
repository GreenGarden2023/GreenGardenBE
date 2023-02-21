using GreeenGarden.Business.Service.ProductService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> getAllProductByCategory([FromQuery] PaginationRequestModel pagingModel, [Required] Guid categoryId)
        {
            var result = await _service.getAllProductByCategory(pagingModel, categoryId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("get-all-products")]
        public async Task<IActionResult> getAllProduct([FromQuery] PaginationRequestModel pagingModel)
        {
            var result = await _service.getAllProduct(pagingModel);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("create-product")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> createProduct([FromForm] ProductCreateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createProduct(model, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("update-product")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.UpdateProduct(model, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

    }
}
