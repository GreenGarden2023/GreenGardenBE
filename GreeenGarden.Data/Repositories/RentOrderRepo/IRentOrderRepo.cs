﻿using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
	public interface IRentOrderRepo : IRepository<TblRentOrder>
	{
		Task<List<TblRentOrder>> GetRentOrders(Guid userID);
        Task<List<TblRentOrder>> GetRentOrdersByGroup(Guid rentOrderGroupID);
        Task<ResultModel> CancelRentOrder(Guid RentOrderID);
	}
}

