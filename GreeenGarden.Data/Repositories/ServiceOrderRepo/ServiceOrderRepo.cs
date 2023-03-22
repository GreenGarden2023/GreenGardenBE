using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
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

        public async Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }
    }
}

