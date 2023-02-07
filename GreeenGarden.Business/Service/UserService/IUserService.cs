using System;
using GreeenGarden.Data.Models.UserModels;

namespace GreeenGarden.Business.Service.UserService
{
	public interface IUserService
	{
        public  Task<string> Register(UserInsertModel userInsertModel);
        public  Task<UserLoginResModel> Login(string userName);
        public Task<UserCurrResModel> GetCurrentUser(string token);
        public  bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        public  string CreateToken(UserLoginResModel user);
    }
}

