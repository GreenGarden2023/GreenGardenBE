using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.UserTreeRepo
{
    public class UserTreeRepo : Repository<TblUserTree>, IUserTreeRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public UserTreeRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<bool> ChangeUserTreeByCustomer(Guid userTreeID, string status)
        {
            var ut = await _context.TblUserTrees.Where(x => x.Id.Equals(userTreeID)).FirstOrDefaultAsync();
            ut.Status = status;
            _context.TblUserTrees.Update (ut);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<DetailUserTreeResModel> GetDetailUserTreeByCustomer(Guid utID)
        {
            var result = new DetailUserTreeResModel();
            var ut = await _context.TblUserTrees.Where(x => x.Id.Equals(utID)).FirstOrDefaultAsync();
            var user = await _context.TblUsers.Where(x => x.Id.Equals(ut.UserId)).FirstOrDefaultAsync();
            var listImg = await getImgUrlByUTID(ut.Id);
            var userRecord = new UserResModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                Mail = user.Mail,
            };
            result.User = userRecord;
            result.UserTrees = new UserTreeForListModel()
            {
                Id = ut.Id,
                TreeName = ut.TreeName,
                Description = ut.Description,
                Quantity = ut.Quantity,
                Status = ut.Status,
                ImgUrl = listImg
            };
            return result;
        }

        public async Task<List<string>> getImgUrlByUTID(Guid UTID)
        {
            var result = new List<string>();
            var listImg = await _context.TblImages.Where(x => x.UserTreeId.Equals(UTID)).ToListAsync();
            foreach (var i in listImg)
            {
                result.Add(i.ImageUrl);
            }
            return result;
        }

        public async Task<ListUserTreeResModel> GetListUserTreeByCustomer(TblUser user)
        {
            var result = new ListUserTreeResModel();
            var listUT =  await _context.TblUserTrees.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            var userRecord = new UserResModel
            {
                Id= user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                Mail = user.Mail
            };
            result.User = userRecord;
            result.UserTrees = new List<UserTreeForListModel>();
            foreach (var ut in listUT)
            {
                var listUrl = await getImgUrlByUTID(ut.Id);
                var utRecord = new UserTreeForListModel()
                {
                    Id= ut.Id,
                    TreeName = ut.TreeName,
                    Description = ut.Description,
                    Quantity = ut.Quantity,
                    Status = ut.Status,
                    ImgUrl = listUrl
                };
                result.UserTrees.Add(utRecord);
            }
            return result;
        }

        public async Task<TblUser> GetTblUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertImg(TblImage entities)
        {
            await _context.TblImages.AddAsync(entities);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateUserTreeByCustomer(UserTreeUpdateModel model)
        {
            var ut = await _context.TblUserTrees.Where(x => x.Id.Equals(model.UserTreeId)).FirstOrDefaultAsync();
            ut.TreeName = model.TreeName;
            ut.Description = model.Description;
            ut.Quantity = model.Quantity;
            _context.TblUserTrees.Update(ut);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
