using GreeenGarden.Business.Service.UserTreeService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreeenGarden.API.Controllers
{
    [Route("user-tree/")]
    [ApiController]
    public class UserTreeController : ControllerBase
    {
        private readonly IUserTreeService _userTreeService;
        public UserTreeController(IUserTreeService userTreeService)
        {
            _userTreeService = userTreeService;
        }
        [HttpPost("create-user-tree")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> CreateUserTree(UserTreeInsertModel userTreeInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _userTreeService.CreateUserTree(token, userTreeInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-user-tree")]
        [SwaggerOperation(Summary = "Get curent user's tree")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetUserTree()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _userTreeService.GetUserTrees(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-user-tree")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> UpdateUserTree(UserTreeUpdateModel userTreeUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _userTreeService.UpdateUserTree(token, userTreeUpdateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-user-tree-status")]
        [SwaggerOperation(Summary = "active/disable")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> UpdateUserTreeStatus(UserTreeStatusModel userTreeStatusModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _userTreeService.UpdateUserTreeStatus(token, userTreeStatusModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

