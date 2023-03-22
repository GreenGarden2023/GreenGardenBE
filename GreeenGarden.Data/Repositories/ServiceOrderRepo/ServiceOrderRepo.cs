using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
	public class ServiceOrderRepo : Repository<TblServiceOrder>, IServiceOrderRepo
	{
		private readonly GreenGardenDbContext _context;
		public ServiceOrderRepo(GreenGardenDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<bool> CheckOrderCode(string Code)
        {
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.OrderCode.Equals(Code)).FirstOrDefaultAsync();
            return order != null;
        }

        public async Task<Page<TblServiceOrder>> GetAllServiceOrders(PaginationRequestModel paginationRequestModel)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<Page<TblServiceOrder>> GetServiceOrderByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.Where(x => x.TechnicianId.Equals(technicianID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<ResultModel> UpdateServiceOrderDeposit(Guid serviceOrderID)
        {
            ResultModel result = new();
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.Id.Equals(serviceOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainAmount -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update service order sucess.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update service order failed.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateServiceOrderRemain(Guid serviceOrderID, double amount)
        {
            ResultModel result = new();
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.Id.Equals(serviceOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {

                order.RemainAmount -= amount;
                if (order.RemainAmount == 0)
                {
                    order.Status = Status.PAID;
                }
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update service order success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update service order failed.";
                return result;
            }
        }
    }
}

