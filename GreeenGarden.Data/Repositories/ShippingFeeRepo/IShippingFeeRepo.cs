using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.ShippingFeeRepo
{
	public interface IShippingFeeRepo : IRepository<TblShippingFee>
	{
		Task<ResultModel> UpdateShippingFee();
	}
}

