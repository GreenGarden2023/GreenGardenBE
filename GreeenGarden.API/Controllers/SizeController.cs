using GreeenGarden.Business.Service.SizeService;
using GreeenGarden.Data.Models.SizeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("size/")]
    [Authorize]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly ISizeService _service;
        public SizeController(ISizeService service)
        {
            _service = service;
        }

        [HttpPost("create-size")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createSize([FromBody] SizeCreateModel sizeCreateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.CreateSize(sizeCreateModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-sizes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSizes()
        {
            Data.Models.ResultModel.ResultModel result = await _service.GetSizes();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-size")]
        public async Task<IActionResult> UpdateSizes(SizeUpdateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.UpdateSizes(model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-size")]
        public async Task<IActionResult> DeleteSizes(Guid sizeID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.DeleteSizes(sizeID, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
