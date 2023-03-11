using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.UserRepo
{
    public interface IUserRepo : IRepository<TblUser>
    {
        public Task<UserLoginResModel> GetUser(string userName);
        public Task<TblUser> UpdateUser(string username, UserUpdateModel userUpdateModel);
        public Task<TblUser> ResetPassword(string email, byte[] passHash, byte[] passSalt);
        public Task<UserCurrResModel> GetCurrentUser(string userName);
        public Task<bool> CheckUserEmail(string email);
    }
}

