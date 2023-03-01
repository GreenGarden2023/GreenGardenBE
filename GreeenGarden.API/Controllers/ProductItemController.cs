using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("product-item/")]
    //[Authorize]
    [ApiController]
    public class ProductItemController : Controller
    {
        private readonly IProductItemService _service;
        public ProductItemController(IProductItemService service)
        {
            _service = service;
        }
        [HttpPost("create-product-item")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> CreateProductItem([FromForm] ProductItemInsertModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.CreateProductItem(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("get-product-item")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductItem([FromQuery] PaginationRequestModel pagingModel, [Required]Guid productID, string? status, string? type)
        {
            var result = await _service.GetProductItem( pagingModel, productID, status, type);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("update-product-item")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateProductItem([FromForm] ProductItemUpdateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.UpdateProductItem(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("update-size-product-item")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateSizeProductItem([FromForm] SizeProductItemUpdateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.UpdateSizeProductItem(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
