using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SaleOrderRepo
{
    public class SaleOrderRepo : Repository<TblSaleOrder>, ISaleOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public SaleOrderRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ResultModel> UpdateSaleOrderStatus(Guid SaleOrderID, string status)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(SaleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = status.Trim().ToLower();
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update order status success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update order status failed.";
                return result;
            }
        }

        public async Task<Page<TblSaleOrder>> GetSaleOrders(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblSaleOrder> listTblOrder = await _context.TblSaleOrders.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<Page<TblSaleOrder>> GetAllSaleOrders(PaginationRequestModel paginationRequestModel)
        {
            Page<TblSaleOrder> listTblOrder = await _context.TblSaleOrders.PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<ResultModel> UpdateSaleOrderDeposit(Guid SaleOrderID)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(SaleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainMoney -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update deposit sucess.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update deposit sucess.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateSaleOrderRemain(Guid saleOrderID, double amount)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(saleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.RemainMoney -= amount;
                if (order.RemainMoney == 0)
                {
                    order.Status = Status.COMPLETED;
                }
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
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

        public async Task<bool> CheckOrderCode(string code)
        {
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.OrderCode.Equals(code)).FirstOrDefaultAsync();
            return order != null;
        }
    }
}

