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

        public async Task<List<UserByRoleResModel>> GetUsersByRole(string role)
        {
            List<UserByRoleResModel> userList = new List<UserByRoleResModel>();
            List<TblUser> userGetList = new List<TblUser>();
            if (role.Equals("admin"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("a56b469d-0f7e-4c3b-bba5-17037581596a"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new UserByRoleResModel
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Admin"
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("technician"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("56d4606a-08d0-4589-bd51-3a195d253ec5"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new UserByRoleResModel
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Technician"
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("customer"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("c98b8768-5827-4e5d-bf3c-3ba67b913d70"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new UserByRoleResModel
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Customer"
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("manager"))
            {

                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("7fb830b9-81c9-4d6e-984c-dfe9a779cf20"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new UserByRoleResModel
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Manager"
                    };
                    userList.Add(resModel);
                }
            }
            else
            {
                userList = null;
            }
            return userList;
        }

        public async Task<UserCurrResModel> GetUserByEmail(string email)
        {
            var query = from u in context.TblUsers
                        join ur in context.TblRoles
                        on u.RoleId equals ur.Id
                        where u.Mail.Equals(email)
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

        public async  Task<bool> UpdateUserStatus(Guid userID, string status)
        {
            var query = from u in context.TblUsers
                        where u.Id.Equals(userID)
                        select new { u };

            TblUser? user = await query.Select(x => x.u).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            user.Status = status;
            _ = _context.Update(user);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserLoginResModel> GetUserByID(Guid userID)
        {
            var query = from u in context.TblUsers
                        join ur in context.TblRoles
                        on u.RoleId equals ur.Id
                        where u.Id.Equals(userID)
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
    }
}


