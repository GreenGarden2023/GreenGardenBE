using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RewardRepo
{
	public class RewardRepo : Repository<TblReward>, IRewardRepo
	{
        private readonly GreenGardenDbContext _context;
		public RewardRepo(GreenGardenDbContext context) :base(context)
		{
            _context = context;
		}

        public async Task<int> GetUserRewardPoint(Guid userID)
        {
            TblReward user = await _context.TblRewards.Where(x => x.UserId.Equals(userID)).FirstOrDefaultAsync();
            if(user != null)
            {
                return (int)user.CurrentPoint;
            }
            else
            {
                return 0;
            }
        }

        public async Task<ResultModel> UpdateUserRewardPoint(string userName, int pointGain, int pointUsed)
        {
            ResultModel? result = new ResultModel();
            TblUser user = await _context.TblUsers.Where(x => x.UserName.Equals(userName)).FirstOrDefaultAsync();
            if (user != null)
            {
                TblReward? reward = await _context.TblRewards.Where(x => x.UserId.Equals(user.Id)).FirstOrDefaultAsync();
                if (reward != null)
                {
                    reward.Total = reward.Total + pointGain;
                    reward.CurrentPoint = reward.CurrentPoint + pointGain - pointUsed;
                    _ = _context.Update(reward);
                    _ = await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "User reward updated.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Can not user reward.";
                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Can not find user.";
                return result;
            }

        }
    }
}

