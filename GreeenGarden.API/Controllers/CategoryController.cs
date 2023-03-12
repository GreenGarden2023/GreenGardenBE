using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            Data.Models.ResultModel.ResultModel result = await _service.getAllCategories(pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-category-by-status")]
        public async Task<IActionResult> getCategoryByStatus([FromQuery] PaginationRequestModel pagingModel, [Required] string status)
        {
            Data.Models.ResultModel.ResultModel result = await _service.GetCategoryByStatus(pagingModel, status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createCategory([FromForm] CategoryCreateModel categoryCreateModel)
        {

            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createCategory(token, categoryCreateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [HttpPost("update-category")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> updateCategory([FromForm] CategoryUpdateModel categoryUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.updateCategory(token, categoryUpdateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-status-category")]
        [Authorize]
        public async Task<IActionResult> changeStatus([FromForm] CategoryUpdateStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}
