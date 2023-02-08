using GreeenGarden.Business.Service.ProductService;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpGet("getAllProductByCategory")]
        public async Task<IActionResult> getAllProductByCategory([FromQuery] PaginationRequestModel pagingModel, Guid categoryId)
        {
            var result = await _service.getAllProductByCategory(pagingModel, categoryId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}
