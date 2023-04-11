using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RewardRepo
{
    public interface IRewardRepo : IRepository<TblReward>
    {
        Task<ResultModel> AddUserRewardPoint(string userName, int pointGain);
        Task<ResultModel> RemoveUserRewardPoint(string userName,int pointUsed);
        Task<ResultModel> AddUserRewardPointByUserID(Guid userID, int pointGain);
        Task<ResultModel> RemoveUserRewardPointByUserID(Guid userID, int pointUsed);
        Task<int> GetUserRewardPoint(Guid userID);
        Task<TblReward> GetUserReward(Guid userID);

    }
}

