using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RentOrderDetailRepo
{
	public class RentOrderDetailRepo : Repository<TblRentOrderDetail> , IRentOrderDetailRepo
	{
		private readonly GreenGardenDbContext _context;
		public RentOrderDetailRepo(GreenGardenDbContext context) : base(context) 
		{
			_context = context;
		}

        public async Task<List<RentOrderDetailResModel>> GetRentOrderDetails(Guid RentOrderId)
        {
			List<TblRentOrderDetail> list = await _context.TblRentOrderDetails.Where(x => x.RentOrderId.Equals(RentOrderId)).ToListAsync();
			List<RentOrderDetailResModel> resultList = new List<RentOrderDetailResModel>();
			foreach(TblRentOrderDetail detail in list)
			{
				RentOrderDetailResModel model = new RentOrderDetailResModel
				{
					ID = detail.Id,
					productItemDetailId = detail.ProductItemDetailId,
					productItemDetailTotalPrice = (double)detail.ProductItemDetailTotalPrice,
					Quantity = (int)detail.Quantity
				};
				resultList.Add(model);
            }
			return resultList;
        }
    }
}

