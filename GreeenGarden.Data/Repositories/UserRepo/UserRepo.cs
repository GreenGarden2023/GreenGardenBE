using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductModel;
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
            var userModel = await query.Select(x => new UserLoginResModel()
            {
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
            var userModel = await query.Select(x => new UserCurrResModel()
            {
                Id = x.u.Id,
                UserName = x.u.UserName,
                FullName = x.u.FullName,
                Address = x.u.Address,
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

            var user = await query.Select(x => x.u).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            if (!String.IsNullOrEmpty(userName) && !userName.Equals(user.UserName))
            {
                user.UserName = userName; 
            }
            if (!String.IsNullOrEmpty(userUpdateModel.FullName) && !userUpdateModel.FullName.Equals(user.FullName))
            {
                user.FullName = userUpdateModel.FullName;
            }
            if (!String.IsNullOrEmpty(userUpdateModel.Address) && !userUpdateModel.Address.Equals(user.Address))
            {
                user.Address = userUpdateModel.Address;
            }
            if (!String.IsNullOrEmpty(userUpdateModel.Phone) && !userUpdateModel.Phone.Equals(user.Phone))
            {
                user.Phone = userUpdateModel.Phone;
            }
            if (!String.IsNullOrEmpty(userUpdateModel.Favorite) && !userUpdateModel.Favorite.Equals(user.Favorite))
            {
                user.Favorite = userUpdateModel.Favorite;
            }
            if (!String.IsNullOrEmpty(userUpdateModel.Mail) && !userUpdateModel.Mail.Equals(user.Mail))
            {
                user.Mail = userUpdateModel.Mail;
            }
            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<bool> CheckUserEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                var check = await _context.TblUsers.Where(x => x.Mail.Equals(email)).FirstOrDefaultAsync();
                if(check != null) { return true; } else { return false; }
            }
            else { return false; }
        }

        public async Task<TblUser> ResetPassword(string email, byte[] passHash, byte[] passSalt)
        {
            var query = from u in context.TblUsers
                        where u.Mail.Equals(email)
                        select new { u };

            var user = await query.Select(x => x.u).FirstOrDefaultAsync();
            if(user != null)
            {
                user.PasswordHash = passHash;
                user.PasswordSalt = passSalt;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }
    }
}


