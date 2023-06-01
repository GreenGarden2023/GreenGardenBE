using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Utilities.Convert;
using Microsoft.EntityFrameworkCore;
using GreeenGarden.Data.Utilities.Convert;

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
            List<UserByRoleResModel>? userList = new();
            List<TblUser> userGetList = new();
            if (role.Equals("admin"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("a56b469d-0f7e-4c3b-bba5-17037581596a"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new()
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Admin",
                        OrderNumber = 0
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("technician"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("56d4606a-08d0-4589-bd51-3a195d253ec5"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    var orderNumber = 0;
                    var orderTakeCare = await _context.TblServiceOrders.Where(x => x.TechnicianId.Equals(user.Id)&&x.Status!=Status.CANCEL).ToListAsync();
                    var comboOrder = await _context.TblTakecareComboOrders.Where(x => x.TechnicianId.Equals(user.Id) && x.Status != Status.CANCEL).ToListAsync();
                    if (orderTakeCare != null) orderNumber += orderTakeCare.Count();
                    if (comboOrder != null) orderNumber += comboOrder.Count();
                    
                    UserByRoleResModel resModel = new()
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Technician",
                        OrderNumber = orderNumber,
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("customer"))
            {
                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("c98b8768-5827-4e5d-bf3c-3ba67b913d70"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new()
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Customer",
                        OrderNumber = 0
                    };
                    userList.Add(resModel);
                }
            }
            else if (role.Equals("manager"))
            {

                userGetList = await _context.TblUsers.Where(x => x.RoleId.Equals(Guid.Parse("7fb830b9-81c9-4d6e-984c-dfe9a779cf20"))).ToListAsync();
                foreach (TblUser user in userGetList)
                {
                    UserByRoleResModel resModel = new()
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Mail,
                        RoleName = "Manager",
                        OrderNumber = 0
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

        public async Task<bool> UpdateUserStatus(Guid userID, string status)
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

        public async Task<Page<UserResByAdminModel>> GetListUser(PaginationRequestModel pagingModel)
        {
            var result = await _context.TblUsers.
                Select(x => new UserResByAdminModel
                {
                    ID = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Address = x.Address,
                    DistrictName = null,
                    Phone= x.Phone,
                    Favorite= x.Favorite,
                    Mail = x.Mail,
                    RoleName = null, 
                    DistrictID = x.DistrictId,
                    Status= x.Status,

                }).
                PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            return result;
        }

        public async Task<string> GetRoleName(Guid userID)
        {
            var user = await _context.TblUsers.Where(x=>x.Id.Equals(userID)).FirstOrDefaultAsync();
            var role = await _context.TblRoles.Where(x=>x.Id.Equals(user.RoleId)).FirstOrDefaultAsync();
            return role.RoleName;
        }

        public async Task<Guid> GetRoleID(string roleName)
        {
            var role = await _context.TblRoles.Where(x => x.RoleName.ToLower().Equals(roleName.ToLower())).FirstOrDefaultAsync();
            return role.Id;
        }

        public async Task<bool> UpdateTblUser(TblUser entity)
        {
            _context.TblUsers.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CheckUserNamePhoneAndMail(string username, string phone, string mail)
        {
            var checkUsername = await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
            var checkPhone = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
            var checkMail = await _context.TblUsers.Where(x => x.Mail.Equals(mail)).FirstOrDefaultAsync();
            if (checkUsername != null)
            {
                return 1;
            }
            if (checkPhone != null)
            {
                return 0;
            }
            if (checkMail != null)
            {
                return 3;
            }
            return 0;
        }

        public async Task<bool> UpdateUserByAdmin(TblUser user)
        {
            _context.TblUsers.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserCurrResModel> GetUserByPhone(string phone)
        {
            var query = from u in context.TblUsers
                        join ur in context.TblRoles
                        on u.RoleId equals ur.Id
                        where u.Phone.Equals(phone)
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

        public async Task<UserResByAdminModel> GetUserCreate(Guid userID)
        {
            var result = await _context.TblUsers.Where(x=>x.Id.Equals(userID)).
                Select(x => new UserResByAdminModel
                {
                    ID = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Address = x.Address,
                    DistrictName = null,
                    Phone = x.Phone,
                    Favorite = x.Favorite,
                    Mail = x.Mail,
                    RoleName = null,
                    DistrictID = x.DistrictId,
                    Status = x.Status,

                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<string> GetFullNameByID(Guid? userID)
        {
            var user = await _context.TblUsers.Where(x => x.Id.Equals(userID)).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            return user.FullName;
        }

        public async Task<Page<UserCurrResModel>> GetUsersByFullname(string fullname, PaginationRequestModel pagingModel)
        {
            var returnResult = new List<UserCurrResModel>();
            var listUsers = await _context.TblUsers.Where(x => x.FullName.Contains(fullname)).ToListAsync();
            foreach (var user in listUsers)
            {
                var record = new UserCurrResModel()
                {
                    Id= user.Id,
                    FullName= user.FullName,
                    Address= user.Address,
                    DistrictID= user.DistrictId,
                    Favorite= user.Favorite,
                    Mail= user.Mail,
                    Phone= user.Phone,
                    UserName= user.UserName
                };
                var role = await _context.TblRoles.Where(x => x.Id.Equals(user.RoleId)).FirstOrDefaultAsync();
                var reward = await _context.TblRewards.Where(x => x.UserId.Equals(user.Id)).FirstOrDefaultAsync();
                record.RoleName = role.RoleName;
                record.CurrentPoint= reward.CurrentPoint;
                returnResult.Add(record);
            }
            var returnResult1 = returnResult.Paginate(pagingModel.curPage, pagingModel.pageSize);

            return returnResult1;
        }

        public async Task<Page<UserCurrResModel>> GetUsersByMail(string mail, PaginationRequestModel pagingModel)
        {
            var returnResult = new List<UserCurrResModel>();
            var listUsers = await _context.TblUsers.Where(x => x.Mail.Contains(mail)).ToListAsync();
            foreach (var user in listUsers)
            {
                var record = new UserCurrResModel()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Address = user.Address,
                    DistrictID = user.DistrictId,
                    Favorite = user.Favorite,
                    Mail = user.Mail,
                    Phone = user.Phone,
                    UserName = user.UserName
                };
                var role = await _context.TblRoles.Where(x => x.Id.Equals(user.RoleId)).FirstOrDefaultAsync();
                var reward = await _context.TblRewards.Where(x => x.UserId.Equals(user.Id)).FirstOrDefaultAsync();
                record.RoleName = role.RoleName;
                record.CurrentPoint = reward.CurrentPoint;
                returnResult.Add(record);
            }
            var returnResult1 = returnResult.Paginate(pagingModel.curPage, pagingModel.pageSize);

            return returnResult1;
        }
    }
}


