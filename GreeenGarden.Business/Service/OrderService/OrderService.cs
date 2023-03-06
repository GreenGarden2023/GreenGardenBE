using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Business.Utilities.Convert;
using System.Security.Claims;

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

        public async Task<ResultModel> addAddendumByOrder(string token, addendumToAddByOrderModel model)
        {
            var result = new ResultModel();
            try
            {
                var user = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                double? totalPrice = 0;
                double? deposit = 0;
                var order = await _orderRepo.GetOrder(model.orderId);
                /********Check*******/
                if (order == null)
                {
                    result.IsSuccess = false;
                    result.Data = "This order dont't exist, please check again!";
                    return result;
                }
                if (!order.UserId.Equals(user.Id))
                {
                    result.IsSuccess = false;
                    result.Data = "This order isn't belong user: " + user.Id;
                    return result;
                }
                if (order.Status != Status.COMPLETED)
                {
                    result.IsSuccess = false;
                    result.Data = "Please pay in full before making a new transaction!";
                    return result;
                }
                foreach (var item in model.sizeProductItems)
                {
                    var sizeProduct = await _orderRepo.GetSizeProductItem(item.sizeProductItemID);
                    if (item.quantity > sizeProduct.Quantity)
                    {
                        result.IsSuccess = false;
                        result.Data = "Product " + sizeProduct.Id + " don't enough quantity!";
                        return result;
                    }
                    if (!sizeProduct.Status.Equals(Status.ACTIVE))
                    {
                        result.IsSuccess = false;
                        result.Data = "Product " + sizeProduct.Id + " don't exist!";
                        return result;
                    }
                    if (model.startDateRent > model.endDateRent || model.startDateRent < DateTime.Now)
                    {
                        result.IsSuccess = false;
                        result.Data = "Datetime not valid!";
                        return result;
                    }
                    totalPrice = totalPrice + (item.quantity * sizeProduct.RentPrice);
                }


                deposit = totalPrice / 100 * 20;

                var addendum = new TblAddendum()
                {
                    Id = Guid.NewGuid(),
                    TransportFee = 0,
                    StartDateRent = model.startDateRent,
                    EndDateRent = model.endDateRent,
                    Status = Status.UNPAID,
                    TotalPrice = totalPrice + deposit,
                    Deposit = deposit,
                    ReducedMoney = 0,
                    OrderId = order.Id,
                    RemainMoney = totalPrice + deposit,
                    Address = model.address
                };
                await _orderRepo.insertAddendum(addendum);

                foreach (var item in model.sizeProductItems)
                {
                    var product = await _orderRepo.GetSizeProductItem(item.sizeProductItemID);
                    var addendumProductItems = new TblAddendumProductItem()
                    {
                        Id = Guid.NewGuid(),
                        SizeProductItemPrice = product.RentPrice,
                        SizeProductItemId = product.Id,
                        AddendumId = addendum.Id,
                        Quantity = item.quantity
                    };
                    await _orderRepo.insertAddendumProductItem(addendumProductItems);
                    await _orderRepo.minusQuantitySizeProductItem(product.Id, item.quantity);
                }
                result.Code = 200;
                result.IsSuccess = true;
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

        public async Task<ResultModel> completeAddendum(string token, Guid addendumID)
        {
            var result = new ResultModel();
            try
            {
                var user = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var tblRole = await _orderRepo.GetRole(user.RoleId);
                if (!tblRole.RoleName.Equals(Commons.MANAGER))
                {
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }

                var tblAddendum = await _orderRepo.getDetailAddendum(addendumID);



                if (tblAddendum.status != Status.PAID)
                {
                    result.IsSuccess = false;
                    result.Message = "Addendum has not been paid yet";
                    return result;
                }
                await _orderRepo.updateStatusAddendum(tblAddendum.id, Status.COMPLETED);
                await _orderRepo.updateStatusOrder(tblAddendum.orderID, Status.COMPLETED);


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = await _orderRepo.getDetailAddendum(tblAddendum.id);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> createOrder(string token, addToOrderModel model)
        {
            var result = new ResultModel();

            try
            {
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));


                /*******************Kiểm tra trùng**************************/
                if (model.rentItems.Count > 1)
                {
                    for (int i = 0; i < model.rentItems.Count; i++)
                    {
                        for (int j = 1; j < model.rentItems.Count; j++)
                        {
                            if (model.rentItems[i].sizeProductItemID.Equals(model.rentItems[j].sizeProductItemID) && i != j)
                            {
                                model.rentItems[i].quantity += model.rentItems[j].quantity;
                                model.rentItems[j].sizeProductItemID = null;
                            }
                        }
                    }
                }
                if (model.saleItems.Count > 1)
                {
                    for (int i = 0; i < model.saleItems.Count; i++)
                    {
                        for (int j = 1; j < model.saleItems.Count; j++)
                        {
                            if (model.saleItems[i].sizeProductItemID.Equals(model.saleItems[j].sizeProductItemID) && i != j)
                            {
                                model.saleItems[i].quantity += model.saleItems[j].quantity;
                                model.saleItems[j].sizeProductItemID = null;
                            }
                        }
                    }
                }
                foreach (var item in model.saleItems)
                {
                    if (item.quantity == 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Product " + item.sizeProductItemID + " has no quantity yet";
                    }
                }
                foreach (var item in model.rentItems)
                {
                    if (item.quantity == 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Product " + item.sizeProductItemID + " has no quantity yet";
                    }
                }
                foreach (var i in model.saleItems)
                {
                    foreach (var j in model.rentItems)
                    {
                        if (i.sizeProductItemID == j.sizeProductItemID)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)i.sizeProductItemID);
                            if (sizeProItem == null)
                            {
                                result.IsSuccess = false;
                                result.Message = "Don't product: " + i.sizeProductItemID;
                                return result;

                            }
                            if ((i.quantity + j.quantity) > sizeProItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Số lượng của sản phẩm: " + i.sizeProductItemID + " trong kho chỉ còn: " + sizeProItem.Quantity + ", đơn hàng của bạn tổng: " + (i.quantity + j.quantity);
                                return result;
                            }
                        }
                    }
                } // kiểm tra 2 quantity

                #region OrderForRent
                if (model.rentItems != null)
                {
                    DateTime startRentDate = ConvertUtil.convertStringToDateTime(model.startRentDate);
                    DateTime endRentDate = ConvertUtil.convertStringToDateTime(model.endRentDate);
                    double rangeDate = (endRentDate - startRentDate).TotalDays;
                    double? totalPrice = 0;
                    double? deposit = 0;
                    double? transportFee = 0;

                    /*******************Kiểm tra điều kiện, tính totalPrice**************************/
                    foreach (var item in model.rentItems)
                    {
                        if (item.sizeProductItemID != null)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item.sizeProductItemID);
                            if (item.quantity > sizeProItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProItem.Id + " don't enough quantity!";
                                return result;
                            }
                            if (sizeProItem.Status.ToLower() != Status.ACTIVE)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProItem.Id + " don't exist!";
                                return result;
                            }

                            if (startRentDate < DateTime.Now.AddDays(3) || startRentDate > DateTime.Now.AddDays(14))
                            {
                                result.IsSuccess = false;
                                result.Message = "startDate must be greater than 3 days and less than 14 days from now!";
                                return result;
                            }
                            if (startRentDate > endRentDate)
                            {
                                result.IsSuccess = false;
                                result.Message = "Datetime not valid!";
                                return result;
                            }
                            totalPrice = totalPrice + (item.quantity * sizeProItem.RentPrice);

                        }
                    }
                    totalPrice = totalPrice * rangeDate;
                    /*******************Tính các khoản liên quan*********************/
                    deposit = totalPrice / 100 * 20;
                    if (totalPrice < 1000000) transportFee = totalPrice / 100 * 5;
                    if (1000000 <= totalPrice && totalPrice < 10000000) transportFee = totalPrice / 100 * 3;
                    if (totalPrice >= 1000000) transportFee = 0;

                    /*******************Add record**********************************/
                    var order = new TblOrder()
                    {
                        Id = Guid.NewGuid(),
                        Status = Status.UNPAID,
                        CreateDate = DateTime.Now,
                        UserId = tblUser.Id,
                        TotalPrice = totalPrice,
                        IsForRent = true,
                    };
                    await _orderRepo.Insert(order);

                    var newAddendum = new TblAddendum()
                    {
                        Id = Guid.NewGuid(),
                        TransportFee = transportFee,
                        StartDateRent = startRentDate,
                        EndDateRent = endRentDate,
                        Status = Status.UNPAID,
                        TotalPrice = totalPrice + deposit + transportFee,
                        Deposit = deposit,
                        ReducedMoney = 0,
                        OrderId = order.Id,
                        RemainMoney = totalPrice + deposit + transportFee,
                        Address = tblUser.Address,
                    };
                    await _orderRepo.insertAddendum(newAddendum);

                    foreach (var item1 in model.rentItems)
                    {
                        if (item1.sizeProductItemID != null)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item1.sizeProductItemID);
                            var addendumProductItems = new TblAddendumProductItem()
                            {
                                Id = Guid.NewGuid(),
                                SizeProductItemPrice = sizeProItem.RentPrice,
                                SizeProductItemId = sizeProItem.Id,
                                AddendumId = newAddendum.Id,
                                Quantity = item1.quantity
                            };
                            await _orderRepo.insertAddendumProductItem(addendumProductItems);
                            await _orderRepo.minusQuantitySizeProductItem((Guid)item1.sizeProductItemID, (int)item1.quantity);
                        }

                    }
                }
                #endregion

                #region OrderForSale
                if (model.saleItems != null)
                {
                    double? totalPriceSale = 0;
                    double? transportFeeSale = 0;

                    foreach (var item in model.saleItems)
                    {
                        if (item.sizeProductItemID != null)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item.sizeProductItemID);
                            if (item.quantity > sizeProItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProItem.Id + " don't enough quantity!";
                                return result;
                            }
                            if (sizeProItem.Status.ToLower() != Status.ACTIVE)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProItem.Id + " don't exist!";
                                return result;
                            }
                            totalPriceSale = totalPriceSale + (item.quantity * sizeProItem.RentPrice);
                        }
                    }

                    /*******************Tính các khoản liên quan*********************/
                    if (totalPriceSale < 1000000) transportFeeSale = totalPriceSale / 100 * 5;
                    if (1000000 <= totalPriceSale && totalPriceSale < 10000000) transportFeeSale = totalPriceSale / 100 * 3;
                    if (totalPriceSale >= 1000000) transportFeeSale = 0;

                    var newOrder = new TblOrder()
                    {
                        Id = Guid.NewGuid(),
                        TotalPrice = totalPriceSale,
                        CreateDate = DateTime.Now,
                        Status = Status.UNPAID,
                        UserId = tblUser.Id,
                        IsForRent = false,
                    };
                    await _orderRepo.Insert(newOrder);

                    var newAddendum = new TblAddendum()
                    {
                        Id = Guid.NewGuid(),
                        TransportFee = transportFeeSale,
                        StartDateRent = null,
                        EndDateRent = null,
                        Deposit = 0,
                        ReducedMoney = 0,
                        TotalPrice = totalPriceSale,
                        Status = Status.UNPAID,
                        OrderId = newOrder.Id,
                        RemainMoney = 0,
                        Address = tblUser.Address,
                    };
                    await _orderRepo.insertAddendum(newAddendum);

                    foreach (var item in model.saleItems)
                    {
                        if (item.sizeProductItemID != null)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item.sizeProductItemID);
                            var addendumProductItems = new TblAddendumProductItem()
                            {
                                Id = Guid.NewGuid(),
                                SizeProductItemPrice = sizeProItem.SalePrice,
                                SizeProductItemId = sizeProItem.Id,
                                AddendumId = newAddendum.Id,
                                Quantity = item.quantity
                            };
                            await _orderRepo.insertAddendumProductItem(addendumProductItems);
                            await _orderRepo.minusQuantitySizeProductItem((Guid)item.sizeProductItemID, (int)item.quantity);
                        }
                    }
                }
                #endregion

                await _orderRepo.removeCart(tblUser.Id);


                result.IsSuccess = true;
                result.Code = 201;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;

            }
            return result;
        }

        public async Task<ResultModel> deleteListOrder(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                await _orderRepo.removeOrder(tblUser.Id);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "success";
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

                result.Code = 200;
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

        public async Task<ResultModel> getDetailOrder(string token, Guid orderId)
        {
            var result = new ResultModel();
            try
            {
                var user = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));

                var listAddendum = await _orderRepo.getOrderDetail(orderId);
                if (!listAddendum.user.userID.Equals(user.Id))
                {
                    result.IsSuccess = false;
                    result.Message = "order: " + orderId + " don't belong you";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = listAddendum;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getListOrderByCustomer(string token)
        {
            var result = new ResultModel();
            try
            {
                var user = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var order = await _orderRepo.GetListOrder(user.Id);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = order;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }

        public async Task<ResultModel> getListOrderByManager(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var tblRole = await _orderRepo.GetRole(tblUser.RoleId);
                if (tblRole.RoleName != Commons.MANAGER)
                {
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var listUser = await _orderRepo.GetListUser();
                var listOrder = new List<listOrder>();
                foreach (var user in listUser)
                {
                    var listOrderTemp = await _orderRepo.GetListOrder(user.Id);
                    listOrder.Add(listOrderTemp);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = listOrder;
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
