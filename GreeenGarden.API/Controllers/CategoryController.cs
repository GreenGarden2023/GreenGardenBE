using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategogyService _service;
        public CategoryController(ICategogyService service)
        {
            _service = service;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> getAllCategories([FromQuery] PaginationRequestModel pagingModel)
        {
            var result = await _service.getAllCategories(pagingModel);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createCategory(string nameCategory, IFormFile file)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createCategory(token, nameCategory, file);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("updateCategory")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> updateCategory([FromBody] Guid CategoryId, string nameCategory, string status, IFormFile file)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.updateCategory(token, CategoryId, nameCategory, status, file);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

    }
}
