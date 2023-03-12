using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SaleOrderRepo
{
	public class SaleOrderRepo : Repository<TblSaleOrder>, ISaleOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public SaleOrderRepo(GreenGardenDbContext context) : base(context)
        {
		}

        public async Task<ResultModel> CancelSaleOrder(Guid SaleOrderID)
        {
            ResultModel result = new ResultModel();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(SaleOrderID)).FirstOrDefaultAsync();
            if (order != null)
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

        public async Task<List<TblSaleOrder>> GetSaleOrders(Guid userID)
        {
            List<TblSaleOrder> listTblOrder = await _context.TblSaleOrders.Where(x => x.UserId.Equals(userID)).ToListAsync();
            return listTblOrder;
        }
    }
}

