﻿using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.RevenueModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RevenueRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.RevenueService
{
    public class RevenueService : IRevenueService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRevenueRepo _revenueRepo;

        public RevenueService(IRevenueRepo revenueRepo)
        {
            _decodeToken = new DecodeToken();
            _revenueRepo = revenueRepo;
        }
        public async Task<ResultModel> GetRevenueByDateRange(string token, RevenueReqByDateModel model)
        {
            var result = new ResultModel();
            try
            {
                DateTime fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                DateTime toDate = ConvertUtil.convertStringToDateTime(model.toDate);

                var tblRentOrder = await _revenueRepo.getTotalRentOrderCompletedByDateRange(fromDate, toDate);
                var tblSaleOrder = await _revenueRepo.getTotalSaleOrderCompletedByDateRange(fromDate, toDate);
                var tblServiceOrder = await _revenueRepo.getTotalServiceOrderCompletedByDateRange(fromDate, toDate);
                var tblTakeCareComboOrder = await _revenueRepo.getTotalServiceComboOrderCompletedByDateRange(fromDate, toDate);

                double? totalRevenue = 0;
                double? rentRevenue=0;
                double? saleRevenue=0;
                double? serviceRevenue = 0;
                double? serviceComboRevenue = 0;

                foreach (var rentOrder in tblRentOrder)
                {
                    totalRevenue += rentOrder.TotalPrice;
                    rentRevenue += rentOrder.TotalPrice;
                }
                foreach (var saleOrder in tblSaleOrder)
                {
                    totalRevenue += saleOrder.TotalPrice;
                    saleRevenue += saleOrder.TotalPrice;
                }
                foreach (var serviceOrder in tblServiceOrder)
                {
                    totalRevenue += serviceOrder.TotalPrice;
                    serviceRevenue += serviceOrder.TotalPrice;
                }
                foreach (var comboOrder in tblTakeCareComboOrder)
                {
                    totalRevenue += comboOrder.TotalPrice;
                    serviceComboRevenue += comboOrder.TotalPrice;
                }
                var newRes = new RevenueResByDateModel
                {
                    totalRevenue = totalRevenue,
                    rentRevenue = rentRevenue,
                    saleRevenue= saleRevenue,
                    serviceRevenue = serviceRevenue,
                    serviceComboRevenue= serviceComboRevenue,
                };


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newRes;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
