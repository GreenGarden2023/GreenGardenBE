using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
                ResultModel result = await _userService.Register(request);
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
                ResultModel result = await _userService.Login(request);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }

        }
        [HttpGet("get-current-user")]
        [Authorize(Roles = "Admin, Customer, Staff, Deliverer, Manager, Technician")]
        public async Task<ActionResult<ResultModel>> GetCurrentUser()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                ResultModel result = await _userService.GetCurrentUser(token);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }
        [HttpGet("get-user-list-by-role")]
        [SwaggerOperation(Summary = "admin/technician/customer/manager")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<ResultModel>> GetUserListByRole(string role)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                ResultModel result = await _userService.GetUsersByRole(token, role);
                return result;
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }
        [HttpGet("get-list-account-by-admin")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<ResultModel>> GetListAccountByAdmin([FromQuery]PaginationRequestModel pagingModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                ResultModel result = await _userService.GetListAccountByAdmin(token, pagingModel);
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
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                ResultModel result = await _userService.UpdateUser(token, userUpdateModel);
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

                ResultModel result = await _eMailService.SendEmailVerificationOTP(email);

                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultModel>> ResetPassword(PasswordResetModel passwordResetModel)
        {
            try
            {
                ResultModel result = await _userService.ResetPassword(passwordResetModel);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
        [HttpPost("verify-register-otp-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultModel>> VerifyOTPCode(OTPVerifyModel oTPVerifyModel)
        {
            try
            {
                ResultModel result = await _userService.VerifyRegisterOTPCode(oTPVerifyModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }
        [HttpPost("update-user-status")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<ResultModel>> UpdateUserStatus(UserUpdateStatusModel userUpdateStatusModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                ResultModel result = await _userService.UpdateUserStatus(token, userUpdateStatusModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }

        }
    }
}

