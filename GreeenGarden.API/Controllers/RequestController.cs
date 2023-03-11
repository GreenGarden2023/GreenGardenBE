using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.RequestService;
using GreeenGarden.Data.Models.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("request/")]
    //[Authorize]
    [ApiController]
    public class RequestController : Controller
    {
        private readonly IRequestService _service;
        public RequestController(IRequestService service)
        {
            _service = service;
        }



        [HttpPost("create-request")]
        public async Task<IActionResult> createRequest([FromForm] RequestCreateModel requestModel)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createRequest(token, requestModel);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

    }
}
