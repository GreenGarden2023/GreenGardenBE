using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class ProductItemController : Controller
    {
        private readonly IProductItemService _service;
        public ProductItemController(IProductItemService service)
        {
            _service = service;
        }



        [HttpGet("GetSizesOfProduct")]
        public async Task<IActionResult> getSizesOfProduct([FromQuery] PaginationRequestModel pagingModel, Guid productId)
        {
            var result = await _service.getSizesOfProduct(pagingModel, productId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



        [HttpGet("GetItemsOfProductSize")]
        public async Task<IActionResult> getAllProductItemByProductItemSize([FromQuery] PaginationRequestModel pagingModel, Guid productSizeId)
        {
            var result = await _service.getAllProductItemByProductItemSize(pagingModel, productSizeId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("GetDetailItem")]
        public async Task<IActionResult> getDetailItem( Guid productItemId)
        {
            var result = await _service.getDetailItem(productItemId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
