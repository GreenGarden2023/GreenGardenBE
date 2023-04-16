using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;

namespace GreeenGarden.Business.Service.UserService
{
    public interface IUserService
    {
        Task<ResultModel> Register(UserInsertModel userInsertModel);
        Task<ResultModel> Login(UserLoginReqModel userLoginReqModel);
        Task<ResultModel> GetCurrentUser(string token);
        Task<ResultModel> UpdateUser(string token, UserUpdateModel userUpdateModel);
        Task<ResultModel> ResetPassword(PasswordResetModel passwordResetModel);
        Task<ResultModel> GetListUserByFullName(string token, string fullName);
        Task<ResultModel> GetUsersByRole(string token, string role);
        Task<ResultModel> VerifyRegisterOTPCode(OTPVerifyModel oTPVerifyModel);
        Task<ResultModel> UpdateUserStatus(string token, UserUpdateStatusModel userUpdateStatusModel);
        Task<ResultModel> GetListAccountByAdmin(string token, PaginationRequestModel pagingModel);
        Task<ResultModel> AssignRole(string token, UserAssignRoleModel model);
        Task<ResultModel> CreateUserByAdmin(string token, UserInsertModel model);

    }
}

