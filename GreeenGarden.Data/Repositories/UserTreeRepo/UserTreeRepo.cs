using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.UserTreeRepo
{
    public class UserTreeRepo : Repository<TblUserTree>, IUserTreeRepo
    {
        private readonly GreenGardenDbContext _context;
        public UserTreeRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ChangeUserTreeStatus(UserTreeStatusModel userTreeStatusModel)
        {
            try
            {
                TblUserTree tblUserTree = await _context.TblUserTrees.Where(x => x.Id.Equals(userTreeStatusModel.Id)).FirstOrDefaultAsync();
                if (tblUserTree != null)
                {
                    if (userTreeStatusModel.Status.Trim().ToLower().Equals("active"))
                    {
                        tblUserTree.Status = TreeStatus.ACTIVE;
                        _ = _context.Update(tblUserTree);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else if (userTreeStatusModel.Status.Trim().ToLower().Equals("disable"))
                    {
                        tblUserTree.Status = TreeStatus.DISABLE;
                        _ = _context.Update(tblUserTree);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TblUserTree>?> GetUserTrees(Guid userID)
        {
            List<TblUserTree> tblUserTrees = await _context.TblUserTrees.Where(x => x.UserId.Equals(userID)).ToListAsync();
            return tblUserTrees.Any() ? tblUserTrees : null;
        }

        public async Task<bool> UpdateUserTree(UserTreeUpdateModel userTreeUpdateModel)
        {
            try
            {
                TblUserTree tblUserTree = await _context.TblUserTrees.Where(x => x.Id.Equals(userTreeUpdateModel.Id)).FirstOrDefaultAsync();
                if (tblUserTree != null)
                {
                    if (userTreeUpdateModel.TreeName != null && !userTreeUpdateModel.TreeName.Equals(tblUserTree.TreeName))
                    {
                        tblUserTree.TreeName = userTreeUpdateModel.TreeName;
                    }
                    if (userTreeUpdateModel.Description != null && !userTreeUpdateModel.Description.Equals(tblUserTree.Description))
                    {
                        tblUserTree.Description = userTreeUpdateModel.Description;
                    }
                    if (userTreeUpdateModel.Status != null && !userTreeUpdateModel.Status.Equals(tblUserTree.Status))
                    {
                        tblUserTree.Status = userTreeUpdateModel.Status;
                    }
                    if (userTreeUpdateModel.Quantity != null && userTreeUpdateModel.Quantity != tblUserTree.Quantity)
                    {
                        tblUserTree.Quantity = userTreeUpdateModel.Quantity;
                    }
                    _ = _context.Update(tblUserTree);
                    _ = await _context.SaveChangesAsync();
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
