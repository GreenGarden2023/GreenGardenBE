using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Business.Service.UserTreeService;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Models.UserTreeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("userTree/")]
    [Authorize]
    [ApiController]
    public class UserTreeController : Controller
    {
        private readonly IUserTreeService _service;
        public UserTreeController(IUserTreeService service)
        {
            _service = service;
        }

        [HttpPost("create-user-tree")]
        public async Task<IActionResult> createUserTree([FromBody] UserTreeCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createUserTree(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-list-user-tree")]
        public async Task<IActionResult> getListUserTree()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getListUserTree(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-detail-user-tree")]
        public async Task<IActionResult> getDetailUserTree( Guid userTreeID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getDetailUserTree(token, userTreeID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-user-tree")]
        public async Task<IActionResult> updateUserTree([FromBody] UserTreeUpdateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.updateUserTree(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("changeStatus")]
        public async Task<IActionResult> changeStatus(Guid userTreeID, string status)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, userTreeID, status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
