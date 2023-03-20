using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.UserRepo
{
    public class UserRepo : Repository<TblUser>, IUserRepo
    {
        private readonly GreenGardenDbContext _context;

        public UserRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<UserLoginResModel> GetUser(string userName)
        {
            var query = from u in context.TblUsers
                        join ur in context.TblRoles
                        on u.RoleId equals ur.Id
                        where u.UserName.Equals(userName)
                        select new { u, ur };
            UserLoginResModel? userModel = await query.Select(x => new UserLoginResModel()
            {
                ID = x.u.Id,
                UserName = x.u.UserName,
                FullName = x.u.FullName,
                PasswordHash = x.u.PasswordHash,
                PasswordSalt = x.u.PasswordSalt,
                RoleName = x.ur.RoleName,
                Email = x.u.Mail
            }).FirstOrDefaultAsync();
            return userModel;
        }

        public async Task<UserCurrResModel> GetCurrentUser(string userName)
        {
            var query = from u in context.TblUsers
                        join ur in context.TblRoles
                        on u.RoleId equals ur.Id
                        where u.UserName.Equals(userName)
                        select new { u, ur };

            UserCurrResModel? userModel = await query.Select(x => new UserCurrResModel()
            {
                Id = x.u.Id,
                UserName = x.u.UserName,
                FullName = x.u.FullName,
                Address = x.u.Address,
                DistrictID = x.u.DistrictId,
                Phone = x.u.Phone,
                Favorite = x.u.Favorite,
                Mail = x.u.Mail,
                RoleName = x.ur.RoleName,
            }).FirstOrDefaultAsync();
            return userModel;
        }

        public async Task<TblUser> UpdateUser(string userName, UserUpdateModel userUpdateModel)
        {
            var query = from u in context.TblUsers
                        where u.UserName.Equals(userName)
                        select new { u };

            TblUser? user = await query.Select(x => x.u).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(userName) && !userName.Equals(user.UserName))
            {
                user.UserName = userName;
            }
            if (!string.IsNullOrEmpty(userUpdateModel.FullName) && !userUpdateModel.FullName.Equals(user.FullName))
            {
                user.FullName = userUpdateModel.FullName;
            }
            if (!string.IsNullOrEmpty(userUpdateModel.Address) && !userUpdateModel.Address.Equals(user.Address))
            {
                user.Address = userUpdateModel.Address;
            }
            if ((userUpdateModel.DistrictID != null) && !userUpdateModel.Address.Equals(user.DistrictId))
            {
                user.DistrictId = userUpdateModel.DistrictID;
            }
            if (!string.IsNullOrEmpty(userUpdateModel.Phone) && !userUpdateModel.Phone.Equals(user.Phone))
            {
                user.Phone = userUpdateModel.Phone;
            }
            if (!string.IsNullOrEmpty(userUpdateModel.Favorite) && !userUpdateModel.Favorite.Equals(user.Favorite))
            {
                user.Favorite = userUpdateModel.Favorite;
            }
            if (!string.IsNullOrEmpty(userUpdateModel.Mail) && !userUpdateModel.Mail.Equals(user.Mail))
            {
                user.Mail = userUpdateModel.Mail;
            }
            _ = _context.Update(user);
            _ = await _context.SaveChangesAsync();
            return user;

        }

        public async Task<bool> CheckUserEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                TblUser? check = await _context.TblUsers.Where(x => x.Mail.Equals(email)).FirstOrDefaultAsync();
                return check != null;
            }
            else { return false; }
        }

        public async Task<TblUser> ResetPassword(string email, byte[] passHash, byte[] passSalt)
        {
            var query = from u in context.TblUsers
                        where u.Mail.Equals(email)
                        select new { u };

            TblUser? user = await query.Select(x => x.u).FirstOrDefaultAsync();
            if (user != null)
            {
                user.PasswordHash = passHash;
                user.PasswordSalt = passSalt;
                _ = _context.Update(user);
                _ = await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }
    }
}


