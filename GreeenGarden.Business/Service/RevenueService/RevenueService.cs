﻿using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.RevenueModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RevenueRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GreeenGarden.Business.Service.RevenueService
{
    public class RevenueService : IRevenueService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRevenueRepo _revenueRepo;
        private readonly IRentOrderDetailRepo _rentOrderDetailRepo;
        private readonly ISaleOrderDetailRepo _saleOrderDetailRepo;
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        private readonly IProductItemRepo _proItemRepo;
        private readonly IImageRepo _imageRepo;
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;

        public RevenueService(IRevenueRepo revenueRepo, IRentOrderDetailRepo rentOrderDetailRepo
            , IProductItemDetailRepo productItemDetailRepo, ISaleOrderDetailRepo saleOrderDetailRepo
            , IProductItemRepo productItemRepo, IImageRepo imageRepo, IProductRepo productRepo
            , ICategoryRepo categoryRepo)
        {
            _decodeToken = new DecodeToken();
            _revenueRepo = revenueRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _productItemDetailRepo = productItemDetailRepo;
            _saleOrderDetailRepo = saleOrderDetailRepo;
            _proItemRepo = productItemRepo;
            _imageRepo = imageRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
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
                var newRess = new List<TblProductItemDetailRevenueResModel>();

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

                foreach (var i in groupedItems)
                {
                    var productItemDetail = await _productItemDetailRepo.GetItemDetailsByID((Guid)i.productItemDetailId);
                    var product = await getInforProductAndProductItem((Guid)i.productItemDetailId);
                    var record = new TblProductItemDetailRevenueResModel();
                    record.productItemDetail = productItemDetail;
                    record.productItemDetail.Product = product;
                    record.quantity = i.quantity;
                    record.revenueProductItemDetail = i.revenueProductItemDetail;
                    newRess.Add(record);
                }
                var newresss = newRess.OrderByDescending(x => x.quantity).ToList();
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newresss;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRentRevenueByDateRange(string token, RevenueReqByDateModel model)
        {
            var result = new ResultModel();
            try
            {

                DateTime fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                DateTime toDate = ConvertUtil.convertStringToDateTime(model.toDate);

                var newRes = new List<ProductItemDetailRevenueResModel>();
                var newRess = new List<TblProductItemDetailRevenueResModel>();
                var tblRentOrder = await _revenueRepo.getTotalRentOrderCompletedByDateRange(fromDate, toDate);

                double? revenue = 0;
                foreach (var rentOrder in tblRentOrder)
                {
                    var rentOrderDetails = await _rentOrderDetailRepo.GetRentOrderDetailsByRentOrderID(rentOrder.Id);
                    foreach (var rentOrderDetail in rentOrderDetails)
                    {
                        var tblItemDetail = await _productItemDetailRepo.Get((Guid)rentOrderDetail.ProductItemDetailId);
                        int dateRange = (rentOrder.EndDateRent - rentOrder.StartDateRent).Days;
                        var itemDetailRecord = new ProductItemDetailRevenueResModel();
                        itemDetailRecord.productItemDetailId = tblItemDetail.Id;
                        itemDetailRecord.quantity = (int)rentOrderDetail.Quantity;
                        itemDetailRecord.revenueProductItemDetail = (rentOrderDetail.RentPricePerUnit * (int)rentOrderDetail.Quantity)*dateRange;
                        newRes.Add(itemDetailRecord);
                        revenue += (rentOrderDetail.RentPricePerUnit * (int)rentOrderDetail.Quantity)* dateRange;
                    }
                }
                var groupedItems = newRes.GroupBy(x => x.productItemDetailId)
                        .Select(g => new ProductItemDetailRevenueResModel
                        {
                            productItemDetailId = g.Key,
                            quantity = g.Sum(x => x.quantity),
                            revenueProductItemDetail = g.Sum(x => x.revenueProductItemDetail)
                        });
                foreach (var i in groupedItems)
                {
                    var productItemDetail = await _productItemDetailRepo.GetItemDetailsByID((Guid)i.productItemDetailId);
                    var product = await getInforProductAndProductItem((Guid)i.productItemDetailId);

                    var record = new TblProductItemDetailRevenueResModel();
                    record.productItemDetail = productItemDetail;
                    record.productItemDetail.Product = product;
                    record.quantity = i.quantity;
                    record.revenueProductItemDetail = i.revenueProductItemDetail;
                    newRess.Add(record);
                }
                var finalResult = new rentRevenueResModel()
                {
                    itemDetailRevenue = newRess,
                    orderNumer = tblRentOrder.Count(),
                    rentRevenue = revenue
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = finalResult;
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
                    rentRevenue += (rentOrder.TotalPrice - rentOrder.TransportFee);
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

        public async Task<ResultModel> GetSaleRevenueByDateRange(string token, RevenueReqByDateModel model)
        {
            var result = new ResultModel();
            try
            {

                DateTime fromDate = ConvertUtil.convertStringToDateTime(model.fromDate);
                DateTime toDate = ConvertUtil.convertStringToDateTime(model.toDate);

                var newRes = new List<ProductItemDetailRevenueResModel>();
                var newRess = new List<TblProductItemDetailRevenueResModel>();
                var tblSaleOrder = await _revenueRepo.getTotalSaleOrderCompletedByDateRange(fromDate, toDate);

                double? revenue = 0;
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
                    revenue += saleOrder.TotalPrice;
                }
                var groupedItems = newRes.GroupBy(x => x.productItemDetailId)
                        .Select(g => new ProductItemDetailRevenueResModel
                        {
                            productItemDetailId = g.Key,
                            quantity = g.Sum(x => x.quantity),
                            revenueProductItemDetail = g.Sum(x => x.revenueProductItemDetail)
                        });
                foreach (var i in groupedItems)
                {
                    var productItemDetail = await _productItemDetailRepo.GetItemDetailsByID((Guid)i.productItemDetailId);
                    var product = await getInforProductAndProductItem((Guid)i.productItemDetailId);

                    var record = new TblProductItemDetailRevenueResModel();
                    record.productItemDetail = productItemDetail;
                    record.productItemDetail.Product = product;
                    record.quantity = i.quantity;
                    record.revenueProductItemDetail = i.revenueProductItemDetail;
                    newRess.Add(record);
                }

                var finalResult = new rentRevenueResModel()
                {
                    itemDetailRevenue = newRess,
                    orderNumer = tblSaleOrder.Count(),
                    rentRevenue = revenue
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = finalResult;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ProductRevenueResModel> getInforProductAndProductItem(Guid productItemDetailId)
        {
            var result = new ProductRevenueResModel();
            try
            {
                var tblItemDetail = await _productItemDetailRepo.Get(productItemDetailId);
                var tblProductItem = await _proItemRepo.Get(tblItemDetail.ProductItemId);
                var tblProduct = await _productRepo.Get(tblProductItem.ProductId);

                var imgProductItem = await _imageRepo.GetImgUrlProductItem(tblProductItem.Id);
                var imgProduct = await _imageRepo.GetImgUrlProduct(tblProduct.Id);

                var itemRecord = new ProductItemRevenueResModel()
                {
                    ImageURL = imgProductItem.ImageUrl,
                    ProductItemId = tblProductItem.Id,
                    ProductItemName = tblProductItem.Name
                };


                result.ProductId = tblProduct.Id;
                result.ProductName = tblProduct.Name;
                result.Status = tblProduct.Status;
                result.IsForRent = tblProduct.IsForRent;
                result.IsForSale = tblProduct.IsForSale;
                result.ImageURL = imgProduct.ImageUrl;
                result.productItem = itemRecord;

            }
            catch (Exception e)
            {
                return null;
            }
            return result;
        }

        public async Task<ResultModel> GetRevenueByMonth()
        {
            var result = new ResultModel();
            try
            {
                var returnResult = new List<revenueByMonthResModel>();
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                for (int i = 1; i <= month; i++)
                {
                    var fromDate = new DateTime(year, i, 1);
                    var toDate = fromDate.AddMonths(1).AddDays(-1);
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
                    var revenueByMonth = new RevenueResByDateModel()
                    {
                        totalRevenue = totalRevenue,
                        rentRevenue = rentRevenue,
                        saleRevenue = saleRevenue,
                        serviceRevenue = serviceRevenue,
                        serviceComboRevenue = serviceComboRevenue,
                    };
                    var recordItem = new revenueByMonthResModel()
                    {
                        month = i,
                        revenues = revenueByMonth
                    };
                    returnResult.Add(recordItem);
                }
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = returnResult;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRevenueInMonth()
        {
            var result = new ResultModel();
            try
            {
                var returnResult = new List<revenueByMonthResModel>();
                int month = DateTime.Now.AddMonths(-1).Month;
                int year = DateTime.Now.Year;
                if (month == 12)
                    year = DateTime.Now.AddYears(-1).Year;
                var fromDate = new DateTime(year, month, 1);
                var toDate = fromDate.AddMonths(1).AddDays(-1);
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
                var revenueByMonth = new RevenueResByDateModel()
                {
                    totalRevenue = totalRevenue,
                    rentRevenue = rentRevenue,
                    saleRevenue = saleRevenue,
                    serviceRevenue = serviceRevenue,
                    serviceComboRevenue = serviceComboRevenue,
                };
                var recordItem = new revenueByMonthResModel()
                {
                    month = month,
                    revenues = revenueByMonth
                };

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = recordItem;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRevenueInDay()
        {
            var result = new ResultModel();
            try
            {
                var returnResult = new List<revenueByMonthResModel>();
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int day = DateTime.Now.Day;
                var fromDate = new DateTime(year, month, day);
                var toDate = fromDate.AddDays(1).AddSeconds(-1);
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
                var revenueByMonth = new RevenueResByDateModel()
                {
                    totalRevenue = totalRevenue,
                    rentRevenue = rentRevenue,
                    saleRevenue = saleRevenue,
                    serviceRevenue = serviceRevenue,
                    serviceComboRevenue = serviceComboRevenue,
                };
                var recordItem = new revenueByMonthResModel()
                {
                    revenues = revenueByMonth
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = recordItem;
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

