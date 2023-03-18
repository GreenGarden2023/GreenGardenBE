using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.ShippingFeeRepo
{
	public class ShippingFeeRepo : Repository<TblShippingFee>, IShippingFeeRepo
	{
		private readonly GreenGardenDbContext _context;
		public ShippingFeeRepo(GreenGardenDbContext context) : base(context)
		{
			_context = context;
		}

        public Task<ResultModel> UpdateShippingFee()
        {
            throw new NotImplementedException();
        }
    }
}

