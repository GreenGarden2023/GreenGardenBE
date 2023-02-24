using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.OrderService
{
    public class OrderService : IOrderService
    {
        //private readonly IMapper _mapper;
        private readonly IOrderRepo _orderRepo;
        private readonly DecodeToken _decodeToken;
        public OrderService(/*IMapper mapper,*/ IOrderRepo orderRepo)
        {
            //_mapper = mapper;
            _orderRepo = orderRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> createOrder(string token, OrderModel model)
        {
            var result = new ResultModel();

            try
            {
                string username = _decodeToken.Decode(token, "username");
                var tblUser = await _orderRepo.getUserByUsername(username);
                double? totalPrice = 0;
                double? deposit = 0;

                foreach (var item in model.Items)
                {
                    var product = await _orderRepo.getProductToCompare(item.productId);
                    if (item.quantity > product.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Data = "Product " + product.Name + " don't enough quantity!";
                        return result;
                    }
                    if (product.Status != Status.ACTIVE)
                    {
                        result.IsSuccess = false;
                        result.Data = "Product " + product.Name + " don't exist!";
                        return result;
                    }
                    if (model.startDate > model.endDate || model.startDate < DateTime.Now)
                    {
                        result.IsSuccess = false;
                        result.Data = "Datetime not valid!";
                        return result;
                    }
                    totalPrice = totalPrice + (item.quantity * product.RentPrice);
                }

                var order = new TblOrder()
                {
                    Id = Guid.NewGuid(),
                    Status = Status.UNPAID,
                    CreateDate = DateTime.Now,
                    UserId = tblUser.Id,
                    TotalPrice = totalPrice,
                };
                await _orderRepo.Insert(order);


                var addendum = new TblAddendum()
                {
                    Id = Guid.NewGuid(),
                    TransportFee = 0,
                    StartDateRent = model.startDate,
                    EndDateRent = model.endDate,
                    Status = Status.UNPAID,
                    TotalPrice = totalPrice,
                    Deposit = totalPrice / 100 * 10,
                    ReducedMoney = 0,
                    OrderId = order.Id,
                    RemainMoney = totalPrice
                };
                await _orderRepo.insertAddendum(addendum);

                foreach (var item in model.Items)
                {
                    var product = await _orderRepo.getProductToCompare(item.productId);
                    var addendumProductItems = new TblAddendumProductItem()
                    {
                        Id = Guid.NewGuid(),
                        ProductItemPrice = product.RentPrice,
                        ProductItemId = product.Id,
                        AddendumId = addendum.Id,
                        Quantity = item.quantity
                    };
                    await _orderRepo.insertAddendumProductItem(addendumProductItems);
                    await _orderRepo.minusQuantityProductItem(product.Id, item.quantity);
                }


                result.IsSuccess = true;
                result.Code = 200;
                result.Data = await _orderRepo.getDetailAddendum(addendum.Id);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;

            }
            return result;
        }

        public async Task<ResultModel> getDetailAddendum(Guid addendumId)
        {
            var result = new ResultModel();
            try
            {
                var detailAddendum = await _orderRepo.getDetailAddendum(addendumId);

                result.Code= 200;
                result.IsSuccess = true;
                result.Data = detailAddendum;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getListAddendum(string token, Guid orderId)
        {
            var result = new ResultModel();
            try
            {

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> payByCashForAddendum(string token, Guid addendumId)
        {
            var result = new ResultModel();
            try
            {
                var addendum = await _orderRepo.GetAddendum(addendumId);
                /*var transaction = new TblTransaction() {
                    Id = Guid.NewGuid(),
                    AddendumId = addendumId,
                    Type = ""
                };*/


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
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
