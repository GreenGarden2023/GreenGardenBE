using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RewardRepo
{
	public interface IRewardRepo : IRepository<TblReward>
	{
		Task<ResultModel> UpdateUserRewardPoint(string userName, int pointGain, int pointUsed);
	}
}

