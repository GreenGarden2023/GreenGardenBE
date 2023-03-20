using System;
using System.Data;
using GreeenGarden.Business.Service.UserTreeService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}

