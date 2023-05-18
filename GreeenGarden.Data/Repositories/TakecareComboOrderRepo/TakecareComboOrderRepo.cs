using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.TakecareComboServiceDetailRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.TakecareComboOrderRepo
{
	public class TakecareComboOrderRepo : Repository<TblTakecareComboOrder>, ITakecareComboOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public TakecareComboOrderRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ChangeTakecareComboOrderStatus(Guid id, string status)
        {
            try
            {
                TblTakecareComboOrder tblTakecareComboOrder = await _context.TblTakecareComboOrders.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
                tblTakecareComboOrder.Status = status;
                _ = _context.Update(tblTakecareComboOrder);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrder(PaginationRequestModel paginationRequestModel)
        {
            Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }
    }
}

