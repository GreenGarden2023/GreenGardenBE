using GreeenGarden.Business.Utilities.Convert;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.OrderRepo;

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
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var orderDetail = await _orderRepo.getDetailOrder(model.orderId, 1, null);
                DateTime startRentDate = ConvertUtil.convertStringToDateTime(model.startDateRent);
                DateTime endRentDate = ConvertUtil.convertStringToDateTime(model.endDateRent);
                var tblLastAddendum = await _orderRepo.getLastAddendum(model.orderId);
                double rangeDate = (endRentDate - startRentDate).TotalDays;
                double? totalPrice = 0;
                double? deposit = 0;
                double? transportFee = 0;

                #region Region: kiểm tra giỏ hàng
                if (tblLastAddendum.Status != Status.COMPLETED)
                {
                    if (tblLastAddendum.Status != Status.PAID)
                    {
                        if (tblLastAddendum.Status != Status.CANCEL)
                        {
                            result.IsSuccess = false;
                            result.Message = "Status order must be: Completed, Paid or Cancel";
                            return result;
                        }
                    }
                }
                var lastAddendumDetail = await _orderRepo.getDetailOrder(tblLastAddendum.OrderId, 2, tblLastAddendum.Id);
                foreach (var item in model.sizeProductItems)
                {
                    foreach (var addendumItem in lastAddendumDetail.order.addendums.LastOrDefault().addendumProductItems)
                    {
                        if (item.sizeProductItemID != addendumItem.sizeProductItems.sizeProductItemID)
                        {
                            result.IsSuccess = true;
                            result.Message = "Item: " + item.sizeProductItemID + " đang không được thuê";
                            return result;
                        }
                        if (item.sizeProductItemID == addendumItem.sizeProductItems.sizeProductItemID)
                        {
                            if (item.quantity > addendumItem.Quantity)
                            {
                                result.IsSuccess = true;
                                result.Message = "Item: " + item.sizeProductItemID + " số lượng gia hạn phải ít hơn số lượng dăng được thuê";
                                return result;
                            }

                        }
                    }
                }



                #endregion

                #region Region: kiểm tra điều kiện item, tính totalPrice 
                foreach (var item in model.sizeProductItems)
                {
                    if (item.sizeProductItemID != null)
                    {
                        var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item.sizeProductItemID);

                        if (startRentDate < DateTime.Now || startRentDate > DateTime.Now.AddDays(14))
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
                #endregion

                #region Region: Tính toán và ghi record
                totalPrice = totalPrice * rangeDate;
                /******************Tính các khoản liên quan********************/
                deposit = totalPrice / 100 * 20;
                if (totalPrice < 1000000) transportFee = (totalPrice / rangeDate) / 100 * 5;
                if (1000000 <= totalPrice && totalPrice < 10000000) transportFee = (totalPrice / rangeDate) / 100 * 3;
                if (totalPrice >= 1000000) transportFee = 0;

                /******************Add record*********************************/

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
                    OrderId = orderDetail.order.orderID,
                    RemainMoney = totalPrice + deposit + transportFee,
                    Address = tblUser.Address,
                };
                await _orderRepo.insertAddendum(newAddendum);

                foreach (var item1 in model.sizeProductItems)
                {
                    if (item1.sizeProductItemID != null)
                    {
                        var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)item1.sizeProductItemID);
                        var addendumProductItems = new TblAddendumProductItem()
                        {
                            Id = Guid.NewGuid(),
                            SizeProductItemPrice = sizeProItem.RentPrice,
                            ProductItemDetailId = sizeProItem.Id,
                            AddendumId = newAddendum.Id,
                            Quantity = item1.quantity
                        };
                        await _orderRepo.insertAddendumProductItem(addendumProductItems);
                        await _orderRepo.minusQuantitySizeProductItem((Guid)item1.sizeProductItemID, (int)item1.quantity);
                    }
                }
                #endregion

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

        public async Task<ResultModel> changeStatusOrder(string token, Guid orderID, string status)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var tblRole = await _orderRepo.GetRole(tblUser.RoleId);
                if (tblRole.RoleName != Commons.MANAGER)
                {
                    result.IsSuccess = false;
                    result.Message = "role valid";
                    return result;
                }

                await _orderRepo.updateStatusOrder(orderID, status);

                result.Code = 201;
                result.IsSuccess = true;
                result.Message = "Succesfully";
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
                var response = new List<Guid>();


                /*******************Kiểm tra trùng**************************/
                #region checkCondition
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
                // kiểm tra 2 quantity cộng lại
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
                }
                #endregion

                #region OrderForRent
                if (model.rentItems != null)
                {
                    if (model.rentItems.Count != 0)
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
                        if (totalPrice < 1000000) transportFee = (totalPrice / rangeDate) / 100 * 20;
                        if (1000000 <= totalPrice && totalPrice < 10000000) transportFee = (totalPrice / rangeDate) / 100 * 10;
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
                        response.Add(order.Id);

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
                                    ProductItemDetailId = sizeProItem.Id,
                                    AddendumId = newAddendum.Id,
                                    Quantity = item1.quantity
                                };
                                await _orderRepo.insertAddendumProductItem(addendumProductItems);
                                await _orderRepo.minusQuantitySizeProductItem((Guid)item1.sizeProductItemID, (int)item1.quantity);
                            }

                        }

                    }

                }
                #endregion

                #region OrderForSale
                if (model.saleItems != null)
                {
                    if (model.saleItems.Count != 0)
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
                        response.Add(newOrder.Id);

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
                                    ProductItemDetailId = sizeProItem.Id,
                                    AddendumId = newAddendum.Id,
                                    Quantity = item.quantity
                                };
                                await _orderRepo.insertAddendumProductItem(addendumProductItems);
                                await _orderRepo.minusQuantitySizeProductItem((Guid)item.sizeProductItemID, (int)item.quantity);
                            }
                        }

                    }

                }
                #endregion

                await _orderRepo.removeCart(tblUser.Id);


                result.IsSuccess = true;
                result.Code = 200;
                result.Data = response;
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

        public async Task<ResultModel> getDetailAddendum(string token, Guid addendumId)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _orderRepo.GetUser(_decodeToken.Decode(token, "username"));
                var tblOrder = await _orderRepo.GetOrder(null, addendumId);
                if (tblOrder.UserId != tblUser.Id)
                {
                    result.IsSuccess = false;
                    result.Message = "This Addendum don't belong user";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = await _orderRepo.getDetailOrder(tblOrder.Id, 2, addendumId);
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

                var listAddendum = await _orderRepo.getDetailOrder(orderId, 1, null);
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
                var order = await _orderRepo.GetListOrderByUserID(user.Id);

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

        public async Task<ResultModel> getListOrderByManager(string token, PaginationRequestModel paginationRequestModel)
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
                var listOrder = await _orderRepo.GetListOrder(paginationRequestModel);
                var response = new List<orderDetail>();
                foreach (var item in listOrder.Results)
                {
                    var orderDetail = await _orderRepo.getDetailOrder(item.Id, 1, null);
                    response.Add(orderDetail);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = response;
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
