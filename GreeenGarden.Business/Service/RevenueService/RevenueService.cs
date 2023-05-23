using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.RevenueModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RevenueRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using Org.BouncyCastle.Utilities;
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
        private readonly IRentOrderDetailRepo _rentOrderDetailRepo;
        private readonly ISaleOrderDetailRepo _saleOrderDetailRepo;
        private readonly IProductItemDetailRepo _productItemDetailRepo;

        public RevenueService(IRevenueRepo revenueRepo, IRentOrderDetailRepo rentOrderDetailRepo
            , IProductItemDetailRepo productItemDetailRepo, ISaleOrderDetailRepo saleOrderDetailRepo)
        {
            _decodeToken = new DecodeToken();
            _revenueRepo = revenueRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _productItemDetailRepo = productItemDetailRepo;
            _saleOrderDetailRepo = saleOrderDetailRepo;
        }

        public async Task<ResultModel> GetBestProductDetailByDateRange(string token, RevenueReqByDateModel model)
        {
            var result = new ResultModel();
            try
            {
                DateTime fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                DateTime toDate = ConvertUtil.convertStringToDateTime(model.toDate);

                var tblRentOrder = await _revenueRepo.getTotalRentOrderCompletedByDateRange(fromDate, toDate);
                var tblSaleOrder = await _revenueRepo.getTotalSaleOrderCompletedByDateRange(fromDate, toDate);

                var newRes = new List<ProductItemDetailRevenueResModel>();

                foreach (var rentOrder in tblRentOrder)
                {
                    var rentOrderDetails = await _rentOrderDetailRepo.GetRentOrderDetailsByRentOrderID(rentOrder.Id);
                    foreach (var rentOrderDetail in rentOrderDetails)
                    {
                        var tblItemDetail = await _productItemDetailRepo.Get((Guid)rentOrderDetail.ProductItemDetailId);

                        var itemDetailRecord = new ProductItemDetailRevenueResModel();
                        itemDetailRecord.productItemDetailId = tblItemDetail.Id;
                        itemDetailRecord.quantity = (int)rentOrderDetail.Quantity;
                        itemDetailRecord.revenueProductItemDetail = rentOrderDetail.RentPricePerUnit * (int)rentOrderDetail.Quantity;
                        newRes.Add(itemDetailRecord);
                    }
                }
                foreach (var saleOrder in tblSaleOrder)
                {
                    var saleOrderDetails = await _saleOrderDetailRepo.GetSaleOrderDetailByOrderId(saleOrder.Id);
                    foreach (var saleOrderDetail in saleOrderDetails)
                    {
                        var tblItemDetail = await _productItemDetailRepo.Get((Guid)saleOrderDetail.ProductItemDetailId);

                        var itemDetailRecord = new ProductItemDetailRevenueResModel();
                        itemDetailRecord.productItemDetailId = tblItemDetail.Id;
                        itemDetailRecord.quantity = (int)saleOrderDetail.Quantity;
                        itemDetailRecord.revenueProductItemDetail = saleOrderDetail.SalePricePerUnit * (int)saleOrderDetail.Quantity;
                        newRes.Add(itemDetailRecord);
                    }
                }
                var groupedItems = newRes.GroupBy(x => x.productItemDetailId)
                        .Select(g => new ProductItemDetailRevenueResModel
                        {
                            productItemDetailId = g.Key,
                            quantity = g.Sum(x => x.quantity),
                            revenueProductItemDetail = g.Sum(x => x.revenueProductItemDetail)
                        });

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = groupedItems;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
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
                double? rentRevenue = 0;
                double? saleRevenue = 0;
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
                    saleRevenue = saleRevenue,
                    serviceRevenue = serviceRevenue,
                    serviceComboRevenue = serviceComboRevenue,
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
