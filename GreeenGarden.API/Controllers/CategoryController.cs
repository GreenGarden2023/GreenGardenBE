using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("category/")]
    //[Authorize]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategogyService _service;
        public CategoryController(ICategogyService service)
        {
            _service = service;
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> getAllCategories([FromQuery] PaginationRequestModel pagingModel)
        {
            var result = await _service.getAllCategories(pagingModel);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get-category-by-status")]
        public async Task<IActionResult> getCategoryByStatus([FromQuery] PaginationRequestModel pagingModel, [Required] string status)
        {
            var result = await _service.GetCategoryByStatus(pagingModel, status);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createCategory([FromForm] CategoryCreateModel categoryCreateModel)
        {
            
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createCategory(token, categoryCreateModel);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("update-category")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> updateCategory([FromForm] CategoryUpdateModel categoryUpdateModel)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.updateCategory(token, categoryUpdateModel);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

    }
}
