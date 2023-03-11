using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.ProductItemModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> CreateProductItem([FromBody] ProductItemModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.CreateProductItem(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("create-product-item-detail")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> CreateProductItemSize([FromBody] ProductItemDetailModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.CreateProductItemDetail(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-product-item")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductItem([FromQuery] PaginationRequestModel pagingModel, [Required] Guid productID, string? status, string? type)
        {
            Data.Models.ResultModel.ResultModel result = await _service.GetProductItem(pagingModel, productID, status, type);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-product-item")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateProductItem([FromBody] ProductItemModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.UpdateProductItem(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-product-item-detail")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateSizeProductItem([FromBody] ProductItemDetailModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.UpdateProductItemDetail(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-product-item-detail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductItemDetail([Required] Guid productItemId, string? sizeProductItemStatus)
        {
            Data.Models.ResultModel.ResultModel result = await _service.GetDetailProductItem(productItemId, sizeProductItemStatus);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
