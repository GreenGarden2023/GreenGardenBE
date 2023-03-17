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



        [HttpGet("get-products-by-category-status")]
        public async Task<IActionResult> getAllProductByStatus([FromQuery] PaginationRequestModel pagingModel, [Required] Guid categoryID, string? status, string? rentSale)
        {
            Data.Models.ResultModel.ResultModel result = await _service.getAllProductByCategoryStatus(pagingModel, categoryID, status, rentSale);
            return result.IsSuccess && result.Code == 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-products")]
        public async Task<IActionResult> getAllProduct([FromQuery] PaginationRequestModel pagingModel)
        {
            Data.Models.ResultModel.ResultModel result = await _service.getAllProduct(pagingModel);
            return result.IsSuccess && result.Code == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-product")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> createProduct([FromForm] ProductCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createProduct(model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-product")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.UpdateProduct(model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("change-status-product")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus([FromForm] ProductUpdateStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.ChangeStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}
