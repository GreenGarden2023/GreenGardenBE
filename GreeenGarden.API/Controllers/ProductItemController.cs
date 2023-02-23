using System.ComponentModel.DataAnnotations;
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
        [HttpGet("get-products-items")]
        public async Task<IActionResult> GetProductItems([FromQuery] PaginationRequestModel pagingModel, [Required] Guid productID,Guid? sizeID ,string? type, string? status)
        {
            var result = await _service.GetProductItems(pagingModel, productID, sizeID, type, status);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
