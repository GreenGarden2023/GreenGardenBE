using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.UserRepo
{
	public class UserRepo : Repository<TblUser>, IUserRepo
	{
		public UserRepo(GreenGardenDbContext context) : base(context)
		{

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
    }
}


