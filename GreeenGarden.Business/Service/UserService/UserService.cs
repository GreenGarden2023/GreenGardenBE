using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.DistrictRepo;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GreeenGarden.Business.Service.UserService
{

    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IEMailService _eMailService;
        private readonly IRewardRepo _rewardRepo;
        private readonly IDistrictRepo _districtRepo;
        public UserService(IUserRepo userRepo, IEMailService eMailService, IRewardRepo rewardRepo, IDistrictRepo districtRepo)
        {
            _userRepo = userRepo;
            _decodeToken = new DecodeToken();
            _eMailService = eMailService;
            _rewardRepo = rewardRepo;
            _districtRepo = districtRepo;
        }

        public UserService()
        {
        }

        public async Task<ResultModel> Login(UserLoginReqModel userLoginReqModel)
        {
            try
            {
                UserLoginResModel userModel = await _userRepo.GetUser(userLoginReqModel.Username);
                if (userModel == null)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "User not found"
                    };

                }
                if (!VerifyPasswordHash(userLoginReqModel.Password, userModel.PasswordHash, userModel.PasswordSalt))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Wrong password"
                    };
                }

                UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(userLoginReqModel.Username);
                int rewardPoint = await _rewardRepo.GetUserRewardPoint(userCurrResModel.Id);
                userCurrResModel.CurrentPoint = rewardPoint;
                string token = CreateToken(userModel);

                LoginResposneModel loginResposneModel = new()
                {
                    Token = token,
                    User = userCurrResModel,
                };
                return new ResultModel()
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = loginResposneModel,
                    Message = "Login Successful"

                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = ex.ToString()
                };
            }
        }

        public async Task<ResultModel> Register(UserInsertModel userInsertModel)
        {
            UserLoginResModel userModelCheck = await _userRepo.GetUser(userInsertModel.UserName);
            if (userModelCheck != null)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "trùng tài khoản"
                };

            }
            var userModeCheckMail = await _userRepo.GetUserByEmail(userInsertModel.Mail);
            if (userModeCheckMail != null)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "trùng mail"
                };

            }
            var userModeCheckPhone = await _userRepo.GetUserByPhone(userInsertModel.Phone);
            if (userModeCheckPhone != null)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "Trùng sdt"
                };

            }
            bool shippingIDCheck = false;
            for (int i = 1; i <= 19; i++)
            {
                if (userInsertModel.DistrictId == i)
                {
                    shippingIDCheck = true;
                }
            }
            if (shippingIDCheck == false)
            {

                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "District ID invalid.",
                };
            }

            try
            {
                CreatePasswordHash(userInsertModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
                TblUser userModel = new()
                {
                    Id = Guid.NewGuid(),
                    UserName = userInsertModel.UserName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = userInsertModel.FullName,
                    Address = userInsertModel.Address,
                    DistrictId = userInsertModel.DistrictId,
                    Phone = userInsertModel.Phone,
                    Favorite = userInsertModel.Favorite,
                    Mail = userInsertModel.Mail,
                    Status = UserStatus.DISABLED
                };
                _ = await _userRepo.Insert(userModel);
                TblReward tblReward = new()
                {
                    CurrentPoint = 0,
                    Total = 0,
                    UserId = userModel.Id
                };
                _ = await _rewardRepo.Insert(tblReward);

                UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(userModel.UserName);
                int rewardPoint = await _rewardRepo.GetUserRewardPoint(userCurrResModel.Id);
                userCurrResModel.CurrentPoint = rewardPoint;
                _ = await _eMailService.SendEmailRegisterVerificationOTP(userModel.Mail, userModel.UserName);
                return new ResultModel()
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = userCurrResModel,
                    Message = "User Registered"
                };

            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = ex.ToString()
                };
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(UserLoginResModel user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleName),
                new Claim("rolename", user.RoleName),
                new Claim("userid", user.ID.ToString()),
                new Claim("username", user.UserName),
                new Claim("email", user.Email),
            };

            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(
                SecretService.SecretService.GetTokenSecret()));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<ResultModel> GetCurrentUser(string token)
        {
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.ADMIN)
                && !userRole.Equals(Commons.CUSTOMER)
                && !userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF)
                && !userRole.Equals(Commons.DELIVERER)
                && !userRole.Equals(Commons.TECHNICIAN))
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "User not allowed"
                };
            }
            try
            {
                string userName = _decodeToken.Decode(token, "username");
                UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(userName);
                int rewardPoint = await _rewardRepo.GetUserRewardPoint(userCurrResModel.Id);
                userCurrResModel.CurrentPoint = rewardPoint;
                return new ResultModel()
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = userCurrResModel,
                    Message = "Get user successful"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = ex.ToString()
                };
            }
        }

        public async Task<ResultModel> UpdateUser(string token, UserUpdateModel userUpdateModel)
        {
            ResultModel result = new();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            string userName = _decodeToken.Decode(token, "username");
            if (!userRole.Equals(Commons.ADMIN)
                && !userRole.Equals(Commons.CUSTOMER)
                && !userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF)
                && !userRole.Equals(Commons.DELIVERER))
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed."
                };
            }
            bool shippingIDCheck = false;
            for (int i = 1; i <= 19; i++)
            {
                if (userUpdateModel.DistrictID == i)
                {
                    shippingIDCheck = true;
                }
            }
            if (userUpdateModel.DistrictID != null && shippingIDCheck == false)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "District ID invalid.";
                return result;
            }
            try
            {
                TblUser updateUser = await _userRepo.UpdateUser(userName, userUpdateModel);
                if (updateUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found.";
                    return result;
                }
                else
                {
                    UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(userName);
                    int rewardPoint = await _rewardRepo.GetUserRewardPoint(userCurrResModel.Id);
                    userCurrResModel.CurrentPoint = rewardPoint;

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = userCurrResModel;
                    result.Message = "Update user successful.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.ToString();
                return result;

            }
        }

        public async Task<ResultModel> ResetPassword(PasswordResetModel passwordResetModel)
        {
            ResultModel result = new();
            try
            {
                ResultModel verifyCode = await _eMailService.VerifyEmailVerificationOTP(passwordResetModel.Email, passwordResetModel.OTPCode);
                if (verifyCode.Code == 200)
                {
                    CreatePasswordHash(passwordResetModel.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    TblUser update = await _userRepo.ResetPassword(verifyCode.Data.ToString(), passwordHash, passwordSalt);
                    if (update != null)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Password reset successfully.";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Password reset failed.";
                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Password reset failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.ToString();
                return result;

            }
        }

        public async Task<ResultModel> GetListUserByFullName(string token, string fullName)
        {
            ResultModel result = new();
            try
            {
                UserLoginResModel tblUser = await _userRepo.GetUser(_decodeToken.Decode(token, "username"));
                if (!tblUser.UserName.Equals(Commons.MANAGER))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "user role invalid!";
                    return result;
                }


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetUsersByRole(string token, string role)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                List<UserByRoleResModel> resList = await _userRepo.GetUsersByRole(role);
                if (resList != null)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = resList;
                    result.Message = "Get user list success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Get user list failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> VerifyRegisterOTPCode(OTPVerifyModel oTPVerifyModel)
        {
            ResultModel result = new();
            try
            {
                ResultModel verify = await _eMailService.VerifyEmailVerificationOTP(oTPVerifyModel.Email, oTPVerifyModel.OTPCode);
                if (verify.IsSuccess == true)
                {
                    UserCurrResModel userCurrResModel = await _userRepo.GetUserByEmail(oTPVerifyModel.Email);
                    _ = await _userRepo.UpdateUserStatus(userCurrResModel.Id, UserStatus.ENABLE);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = userCurrResModel;
                    result.Message = "Code verified successfully.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Code verified failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> UpdateUserStatus(string token, UserUpdateStatusModel userUpdateStatusModel)
        {

            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.ADMIN)
                && !userRole.Equals(Commons.MANAGER))
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed."
                };
            }
            if (!userUpdateStatusModel.Status.Equals(UserStatus.ENABLE))
            {
                if (!userUpdateStatusModel.Status.Equals(UserStatus.DISABLED))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Status unknown."
                    };

                }
            }
            ResultModel result = new();
            try
            {
                bool update = await _userRepo.UpdateUserStatus(userUpdateStatusModel.UserID, userUpdateStatusModel.Status);
                if (update == true)
                {
                    UserLoginResModel user = await _userRepo.GetUserByID(userUpdateStatusModel.UserID);
                    result.Code = 200;
                    result.Data = user;
                    result.IsSuccess = true;
                    result.Message = "Update user status sucessful.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update user status failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> GetListAccountByAdmin(string token, PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                    if (!userRole.Equals(Commons.ADMIN) &&
                        !userRole.Equals(Commons.MANAGER))
                    {
                        return new ResultModel()
                        {
                            IsSuccess = false,
                            Message = "User not allowed"
                        };
                    }
                }
                else
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                var pageUser = await _userRepo.GetListUser(pagingModel);

                foreach (var i in pageUser.Results)
                {
                    var user = await _userRepo.Get(i.ID);
                    var roleName = await _userRepo.GetRoleName(i.ID);
                    var districtName = await _districtRepo.GetNameDistrict((int)user.DistrictId);
                    i.RoleName = roleName;
                    i.DistrictName = districtName;
                }


                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(pageUser.PageSize)
                    .CurPage(pageUser.CurrentPage)
                    .RecordCount(pageUser.RecordCount)
                    .PageCount(pageUser.PageCount);

                var newres = new UserResult()
                {
                    Paging= paging,
                    Users = pageUser.Results
                };

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newres;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> AssignRole(string token, UserAssignRoleModel model)
        {
            var result = new ResultModel();
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                    if (!userRole.Equals(Commons.ADMIN) &&
                        !userRole.Equals(Commons.MANAGER))
                    {
                        return new ResultModel()
                        {
                            IsSuccess = false,
                            Message = "User not allowed"
                        };
                    }
                }
                else
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                var roleID = await _userRepo.GetRoleID(model.RoleName);
                var tblUser = await _userRepo.Get(model.UserID);
                tblUser.RoleId = roleID;
                await _userRepo.UpdateTblUser(tblUser);

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cập nhật role thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateUserByAdmin(string token, UserInsertModel model)
        {
            var result = new ResultModel();
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                    if (!userRole.Equals(Commons.ADMIN) &&
                        !userRole.Equals(Commons.MANAGER))
                    {
                        return new ResultModel()
                        {
                            IsSuccess = false,
                            Message = "User not allowed"
                        };
                    }
                }
                else
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                if (model == null)
                { 
                    result.IsSuccess = false;
                    result.Message = "Data null";
                    return result;
                }
                    var checkUser = await _userRepo.CheckUserNamePhoneAndMail(model.UserName, model.Phone, model.Mail);
                    if (checkUser != 0)
                    {
                        if (checkUser == 1) result.Message = "Tên tài khoản đã tồn tại";
                        if (checkUser == 2) result.Message = "Số điện thoại đã đăng ký";
                        if (checkUser == 3) result.Message = "Mail đã tồn tại";
                        result.IsSuccess = false;
                        return result;
                    }
                    if (model.DistrictId <= 19)
                    {

                    }
                var roleID = await _userRepo.GetRoleID(model.RoleName);
                    CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    TblUser userModel = new()
                    {
                        Id = Guid.NewGuid(),
                        UserName = model.UserName,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        FullName = model.FullName,
                        Address = model.Address,
                        DistrictId = model.DistrictId,
                        Phone = model.Phone,
                        Favorite = model.Favorite,
                        Mail = model.Mail,
                        RoleId = roleID,
                        Status = UserStatus.ENABLE
                    };
                await _userRepo.Insert(userModel);

                var newReward = new TblReward()
                {
                    Id = Guid.NewGuid(),
                    CurrentPoint = 0,
                    Total = 0,
                    UserId = userModel.Id,
                };
                await _rewardRepo.Insert(newReward);


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = await _userRepo.GetCurrentUser(userModel.UserName);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateUserByAdmin(string token, UserUpdateByAdminModel model)
        {
            ResultModel result = new();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.ADMIN)
                && !userRole.Equals(Commons.MANAGER))
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed."
                };
            }
            bool shippingIDCheck = false;
            for (int i = 1; i <= 19; i++)
            {
                if (model.DistrictID == i)
                {
                    shippingIDCheck = true;
                }
            }
            if (model.DistrictID != null && shippingIDCheck == false)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "District ID invalid.";
                return result;
            }
            try
            {
                var roleID = await _userRepo.GetRoleID(model.RoleName);

                var newTblUser = await _userRepo.Get(model.UserID);
                newTblUser.FullName = model.FullName;
                newTblUser.Address = model.Address;
                newTblUser.DistrictId = model.DistrictID;
                newTblUser.Phone = model.Phone;
                newTblUser.Favorite = model.Favorite;
                newTblUser.RoleId = roleID;


                var updateUser = await _userRepo.UpdateUserByAdmin(newTblUser);
                if (updateUser == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found.";
                    return result;
                }
                else
                {
                    UserCurrResModel userCurrResModel = await _userRepo.GetCurrentUser(newTblUser.UserName);
                    int rewardPoint = await _rewardRepo.GetUserRewardPoint(userCurrResModel.Id);
                    userCurrResModel.CurrentPoint = rewardPoint;

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = userCurrResModel;
                    result.Message = "Update user successful.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.ToString();
                return result;
            }
        }
    }
}

