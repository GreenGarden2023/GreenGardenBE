using System;
using System.Net.NetworkInformation;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboOrder;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Utilities.Convert;
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

        public async Task<bool> CancelOrder(Guid orderID, string cancelReason, Guid cancelBy)
        {
            try
            {
                TblTakecareComboOrder tblTakecareComboOrder = await _context.TblTakecareComboOrders.Where(x => x.Id.Equals(orderID)).FirstOrDefaultAsync();
                tblTakecareComboOrder.Status = Status.CANCEL;
                tblTakecareComboOrder.Description = cancelReason;
                tblTakecareComboOrder.CancelBy = cancelBy;
                _ = _context.Update(tblTakecareComboOrder);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrder(PaginationRequestModel paginationRequestModel, string status)
        {
            if (status.Trim().ToLower().Equals("all"))
            {
                Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;
            }
            else
            {
                Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x => x.Status.Trim().ToLower().Equals(status)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;
            }
        }

        public async Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrderForCustomer(PaginationRequestModel paginationRequestModel, string status, Guid userID)
        {
            if (status.Trim().ToLower().Equals("all"))
            {
                /*var query = from service in _context.TblTakecareComboServices
                            join order in _context.TblTakecareComboOrders
                            on service.Id equals order.TakecareComboServiceId
                            where service.UserId == userID
                            select new { Order = order };
                var result = query.AsNoTracking().PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return result;*/

                var tblComboServices = await _context.TblTakecareComboServices.Where(x => x.UserId == userID).ToListAsync();
                var tblComboOrders = new List<TblTakecareComboOrder>();
                foreach (var comboService in tblComboServices)
                {
                    var comboOrder = await _context.TblTakecareComboOrders.Where(x => x.TakecareComboServiceId.Equals(comboService.Id)).FirstOrDefaultAsync();
                    if (comboOrder!=null)
                    {
                        tblComboOrders.Add(comboOrder);
                    }
                };

                var result = tblComboOrders.Paginate(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return result;

                /*Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x=>x.UserId.Equals(userID)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;*/
            }
            else
            {
                var tblComboServices = await _context.TblTakecareComboServices.Where(x => x.UserId == userID).ToListAsync();
                var tblComboOrders = new List<TblTakecareComboOrder>();
                foreach (var comboService in tblComboServices)
                {
                    var comboOrder = await _context.TblTakecareComboOrders.Where(x => x.TakecareComboServiceId.Equals(comboService.Id)&& x.Status.Trim().ToLower().Equals(status)).FirstOrDefaultAsync();
                    if (comboOrder != null)
                    {
                        tblComboOrders.Add(comboOrder);
                    }
                };

                var result = tblComboOrders.Paginate(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return result;
                /*Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x => x.Status.Trim().ToLower().Equals(status) && x.UserId.Equals(userID)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;*/
            }
        }

        public async Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrderForTech(PaginationRequestModel paginationRequestModel, TakecareComboOrderTechnicianReqModel model)
        {
            if (model.status.Trim().ToLower().Equals("all"))
            {
                Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x => x.TechnicianId.Equals(model.technicianId)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;
            }
            else
            {
                Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x => x.Status.Trim().ToLower().Equals(model.status) && x.TechnicianId.Equals(model.technicianId)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
                return listTblOrder;
            }
        }

        public async Task<ResultModel> UpdateOrderDeposit(Guid orderID)
        {
            ResultModel result = new();
            try
            {
                TblTakecareComboOrder tblTakecareComboOrder = await _context.TblTakecareComboOrders.Where(x => x.Id.Equals(orderID)).FirstOrDefaultAsync();
                tblTakecareComboOrder.Status = Status.READY;
                tblTakecareComboOrder.RemainAmount = (double)(tblTakecareComboOrder.RemainAmount - tblTakecareComboOrder.Deposit);
                _ = _context.Update(tblTakecareComboOrder);
                _ = await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Update order deposit success.";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> UpdateOrderRemain(Guid orderID, double payAmount)
        {
            ResultModel result = new();
            try
            {
                TblTakecareComboOrder tblTakecareComboOrder = await _context.TblTakecareComboOrders.Where(x => x.Id.Equals(orderID)).FirstOrDefaultAsync();
                tblTakecareComboOrder.RemainAmount -= payAmount;
                if (tblTakecareComboOrder.RemainAmount == 0)
                {
                    tblTakecareComboOrder.Status = Status.PAID;
                }
                _ = _context.Update(tblTakecareComboOrder);
                _ = await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Update order remain amount success.";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }
    }
}

