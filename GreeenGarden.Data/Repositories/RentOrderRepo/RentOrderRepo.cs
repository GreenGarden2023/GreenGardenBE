using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
	public class RentOrderRepo : Repository<TblRentOrder>, IRentOrderRepo
	{
		private readonly GreenGardenDbContext _context;
		public RentOrderRepo(GreenGardenDbContext context) :base(context)
		{
			_context = context;
		}

        public async Task<ResultModel> CancelRentOrder(Guid RentOrderID)
        {
			ResultModel result = new ResultModel();
			TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(RentOrderID)).FirstOrDefaultAsync();
			if(order != null)
			{
				order.Status = Status.CANCEL;
				_ = _context.Update(order);
				_ = _context.SaveChangesAsync();
				result.Code = 200;
				result.IsSuccess = true;
				result.Message = "Cancel rent order sucess.";
				return result;
			}
			else
			{
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Cancel rent order failed.";
                return result;
            }
        }

        public async Task<List<TblRentOrder>> GetRentOrders(Guid userID)
		{
			List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.UserId.Equals(userID)).ToListAsync();
			return listTblOrder;
		}
	}
}

