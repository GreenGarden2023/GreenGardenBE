using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RentOrderGroupRepo
{
	public interface IRentOrderGroupRepo : IRepository<TblRentOrderGroup>
	{
		Task<ResultModel> UpdateRentOrderGroup(Guid rentOrderGroupID, double newRentOrderAmount);
		Task<List<TblRentOrderGroup>> GetRentOrderGroup(Guid userID);
	}
}

