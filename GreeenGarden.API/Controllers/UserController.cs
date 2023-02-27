using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Data.Models.EmailCodeVerifyModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GreeenGarden.API.Controllers
{
    [Route("user/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEMailService _eMailService;
        public UserController(IUserService userService, IEMailService eMailService)
        {
            _userService = userService;
            _eMailService = eMailService;
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
        [HttpPost("update-user")]
        public async Task<ActionResult<ResultModel>> Update(UserUpdateModel userUpdateModel)
        {
            try
            {
                string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
                var result = await _userService.UpdateUser(token, userUpdateModel);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }

        }
        [HttpPost("send-email-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultModel>> SendEmailVerificationCode(string email)
        {
            try
            {

                var result =  await _eMailService.SendEmailVerificationOTP(email);

                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultModel>> ResetPassword([FromForm] PasswordResetModel passwordResetModel)
        {
            try
            {
                var result = await _userService.ResetPassword(passwordResetModel);

                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }

    }
}

