using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.SubProductService;
using GreeenGarden.Data.Models.SubProductModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class ProductSizeController : Controller
    {
        private readonly ISubProductService _service;
        public ProductSizeController(ISubProductService service)
        {
            _service = service;
        }



        [HttpPost("CreateProductSize")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createCategory([FromForm] SizeItemRequestModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createProductSize(model, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
