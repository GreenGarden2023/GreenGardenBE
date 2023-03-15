using System.Net.NetworkInformation;
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

        public async Task<ResultModel> UpdateRentOrderStatus(Guid RentOrderID, string status)
        {
			ResultModel result = new ResultModel();
			TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(RentOrderID)).FirstOrDefaultAsync();
			if(order != null)
			{
				order.Status = status.Trim().ToLower();
				_ = _context.Update(order);
				_ = await _context.SaveChangesAsync();
				result.Code = 200;
				result.IsSuccess = true;
				result.Message = "Update rent order status success.";
				return result;
			}
			else
			{
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update rent order status failed.";
                return result;
            }
        }

        public async Task<List<TblRentOrder>> GetRentOrders(Guid userID)
		{
			List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.UserId.Equals(userID)).ToListAsync();
			return listTblOrder;
		}

        public async Task<List<TblRentOrder>> GetRentOrdersByGroup(Guid rentOrderGroupID)
        {
            List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.RentOrderGroupId.Equals(rentOrderGroupID)).ToListAsync();
			if (listTblOrder.Any())
			{
                return listTblOrder;
			}
			else
			{
				return null;
			}
        }

        public async Task<ResultModel> UpdateRentOrderDeposit(Guid rentOrderID)
        {
            ResultModel result = new ResultModel();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(rentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainMoney -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = order.Deposit;
                result.Message = "Update rent order status success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update rent order status failed.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateRentOrderRemain(Guid rentOrderID, double amount)
        {
            ResultModel result = new ResultModel();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(rentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {

                order.RemainMoney -= amount;
                if (order.RemainMoney == 0)
                {
                    order.Status = Status.PAID;
                }
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update rent order status success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update rent order status failed.";
                return result;
            }
        }

        public async Task<bool> CheckOrderCode(string Code)
        {
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.OrderCode.Equals(Code)).FirstOrDefaultAsync();
            if (order != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}

