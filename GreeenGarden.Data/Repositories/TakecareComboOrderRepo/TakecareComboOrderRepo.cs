﻿using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
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

        public async Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrder(PaginationRequestModel paginationRequestModel, string status)
        {
            Page<TblTakecareComboOrder> listTblOrder = await _context.TblTakecareComboOrders.Where(x => x.Status.Trim().ToLower().Equals(status)).OrderByDescending(x => x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
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

