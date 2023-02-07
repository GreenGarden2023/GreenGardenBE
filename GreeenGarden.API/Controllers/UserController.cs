using System;
using System.Data;
using System.Security.Cryptography;
using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserModels;
using Microsoft.AspNet.Identity;
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
    public async Task<ActionResult<TblUser>> Register(UserInsertModel request)
    {
        try
        {
            var result = await _userService.Register(request);
            return Ok(result);
        }
        catch (Exception e)
        {

            return BadRequest(e.ToString());
        }
    }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginReqModel request)
        {
            var user = await _userService.Login(request.Username);

            if (user == null || user.UserName != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!_userService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = _userService.CreateToken(user);;
            return Ok(token);

        }
        [HttpGet("get-current-user")]
        [Authorize(Roles = "Admin, Customer, Staff, Deliverer, Manager")]
        public async Task<IActionResult> abc( )
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                UserCurrResModel userCurrResModel = await _userService.GetCurrentUser(token);
                return Ok(userCurrResModel);
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }

    }
}

