using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
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



        [HttpGet("get-product-sizes")]
        public async Task<IActionResult> getSizesOfProduct([FromQuery] PaginationRequestModel pagingModel, Guid productId)
        {
            var result = await _service.getSizesOfProduct(pagingModel, productId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



        [HttpGet("get-product-size-items")]
        public async Task<IActionResult> getAllProductItemByProductItemSize([FromQuery] PaginationRequestModel pagingModel, Guid productSizeId)
        {
            var result = await _service.getAllProductItemByProductItemSize(pagingModel, productSizeId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("get-item-detail")]
        public async Task<IActionResult> getDetailItem( Guid productItemId)
        {
            var result = await _service.getDetailItem(productItemId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("create-product-item")]
        public async Task<IActionResult> createProductItem([FromForm] ProductItemCreateRequestModel model,[FromQuery] IList<IFormFile> imgFile)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createProductItem(model, imgFile, token);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
