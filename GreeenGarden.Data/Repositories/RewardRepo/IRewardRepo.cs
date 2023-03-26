using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RewardRepo
{
    public interface IRewardRepo : IRepository<TblReward>
    {
        Task<ResultModel> UpdateUserRewardPoint(string userName, int pointGain, int pointUsed);
        Task<ResultModel> UpdateUserRewardPointByUserID(Guid userID, int pointGain, int pointUsed);
        Task<int> GetUserRewardPoint(Guid userID);
        Task<TblReward> GetUserReward(Guid userID);

    }
}

