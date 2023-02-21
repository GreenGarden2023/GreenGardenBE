using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("user/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ResultModel>> Register(UserInsertModel request)
        {
            try
            {
                var result = await _userService.Register(request);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<ResultModel>> Login(UserLoginReqModel request)
        {
            try
            {
                var result = await _userService.Login(request);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }

        }
        [HttpGet("get-current-user")]
        [Authorize(Roles = "Admin, Customer, Staff, Deliverer, Manager")]
        public async Task<ActionResult<ResultModel>> GetCurrentUser()
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                var result = await _userService.GetCurrentUser(token);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }

    }
}

