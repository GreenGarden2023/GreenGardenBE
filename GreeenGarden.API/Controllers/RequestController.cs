using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Business.Service.RequestService;
using GreeenGarden.Data.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("request/")]
    [Authorize]
    [ApiController]
    public class RequestController : Controller
    {
        private readonly IRequestService _service;
        public RequestController(IRequestService service)
        {
            _service = service;
        }


        [HttpPost("create-request")]
        public async Task<IActionResult> CreateRequest(RequestCreateModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.CreateRequest(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("get-list-request-by-customer")]
        public async Task<IActionResult> GetListRequest()
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.GetListRequest(token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatus(RequestUpdateStatusModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.ChangeStatus(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
