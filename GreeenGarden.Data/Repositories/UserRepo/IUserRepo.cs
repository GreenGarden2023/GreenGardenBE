using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.UserRepo
{
    public interface IUserRepo : IRepository<TblUser>
    {
        public Task<UserLoginResModel> GetUser(string userName);
        public Task<UserLoginResModel> GetUserByID(Guid userID);
        public Task<List<UserByRoleResModel>> GetUsersByRole(string role);
        public Task<TblUser> UpdateUser(string username, UserUpdateModel userUpdateModel);
        public Task<bool> UpdateUserByAdmin(TblUser user);
        public Task<TblUser> ResetPassword(string email, byte[] passHash, byte[] passSalt);
        public Task<UserCurrResModel> GetCurrentUser(string userName);
        public Task<bool> CheckUserEmail(string email);
        public Task<UserCurrResModel> GetUserByEmail(string email);
        public Task<UserCurrResModel> GetUserByPhone(string phone);
        public Task<Page<UserCurrResModel>> GetUsersByFullname(string fullname, PaginationRequestModel pagingModel);
        public Task<Page<UserCurrResModel>> GetUsersByMail(string mail, PaginationRequestModel pagingModel);
        public Task<string> GetFullNameByID(Guid? userID );
        public Task<bool> UpdateUserStatus(Guid userID, string status);
        public Task<string> GetRoleName(Guid userID);
        public Task<Guid> GetRoleID(string roleName);
        public Task<bool> UpdateTblUser(TblUser entity);
        public Task<int> CheckUserNamePhoneAndMail(string username, string phone, string mail);
        public Task<Page<UserResByAdminModel>> GetListUser(PaginationRequestModel pagingModel);
        public Task<UserResByAdminModel> GetUserCreate(Guid userID);
    }
}

