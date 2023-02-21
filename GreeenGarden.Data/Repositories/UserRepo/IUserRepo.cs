using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.UserRepo
{
    public interface IUserRepo : IRepository<TblUser>
    {
        public Task<UserLoginResModel> GetUser(string userName);
        public Task<UserCurrResModel> GetCurrentUser(string userName);
    }
}

