using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;

namespace GreeenGarden.Business.Service.UserService
{
    public interface IUserService
    {
        public Task<ResultModel> Register(UserInsertModel userInsertModel);
        public Task<ResultModel> Login(UserLoginReqModel userLoginReqModel);
        public Task<ResultModel> GetCurrentUser(string token);
        public Task<ResultModel> UpdateUser(string token, UserUpdateModel userUpdateModel);
        public Task<ResultModel> ResetPassword(PasswordResetModel passwordResetModel);
    }
}

