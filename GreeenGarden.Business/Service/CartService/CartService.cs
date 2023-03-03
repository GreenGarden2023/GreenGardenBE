﻿using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;
using System.Collections.Generic;

namespace GreeenGarden.Business.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ICartRepo _cartRepo;

        public CartService(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> AddToCart(string token, AddToCartModel model)
        {
            var result = new ResultModel();
            try
            {
                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                var modelResponse = new CartShowModel();
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                if (await _cartRepo.GetCart(user.Id) == null)
                {
                    var cartTemp = new TblCart()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Status = Status.ACTIVE,
                    };
                    await _cartRepo.Insert(cartTemp);
                }

                var cart = await _cartRepo.GetCart(user.Id);
                var cartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                foreach (var item in cartDetail)
                {
                    await _cartRepo.RemoveCartDetail(item);
                }

                modelResponse.rentItems = new List<ItemRequest>();
                modelResponse.saleItems = new List<ItemRequest>();

                if (model.rentItems != null)
                {
                    foreach (var item in model.rentItems)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.sizeProductItemID);
                        if (sizeProductItem.Quantity < item.quantity || sizeProductItem.Status.ToLower() != Status.ACTIVE || sizeProductItem.RentPrice == 0)
                        {
                            result.Code = 400;
                            result.IsSuccess = false;
                            result.Message = "Sản phẩm " + item.sizeProductItemID + " không còn đủ số lượng!";
                            return result;
                        }
                        var newCartDetail = new TblCartDetail()
                        {
                            Id = Guid.NewGuid(),
                            SizeProductItemId = item.sizeProductItemID,
                            Quantity = item.quantity,
                            CartId = cart.Id,
                            IsForRent = true
                        };
                        await _cartRepo.AddProductItemToCart(newCartDetail);
                        //show
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItemID = item.sizeProductItemID,
                            quantity = item.quantity,
                            unitPrice = sizeProductItem.RentPrice
                        };
                        modelResponse.rentItems.Add(ItemRequest);
                        totalRentPriceCart += sizeProductItem.RentPrice * item.quantity;
                    }
                }

                if (model.saleItems != null)
                {
                    foreach (var item in model.saleItems)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.sizeProductItemID);
                        if (sizeProductItem.Quantity < item.quantity || sizeProductItem.Status.ToLower() != Status.ACTIVE || sizeProductItem.SalePrice == 0)
                        {
                            result.Code = 400;
                            result.IsSuccess = false;
                            result.Message = "Sản phẩm " + item.sizeProductItemID + " không còn đủ số lượng!";
                            return result;
                        }
                        var newCartDetail = new TblCartDetail()
                        {
                            Id = Guid.NewGuid(),
                            SizeProductItemId = item.sizeProductItemID,
                            Quantity = item.quantity,
                            CartId = cart.Id,
                            IsForRent = false
                        };
                        await _cartRepo.AddProductItemToCart(newCartDetail);
                        //show
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItemID = item.sizeProductItemID,
                            quantity = item.quantity,
                            unitPrice = sizeProductItem.SalePrice
                        };
                        modelResponse.saleItems.Add(ItemRequest);
                        totalSalePriceCart += sizeProductItem.SalePrice * item.quantity;
                    }
                }

                modelResponse.totalRentPrice = totalRentPriceCart;
                modelResponse.totalSalePrice = totalSalePriceCart;
                modelResponse.totalPrice = totalSalePriceCart+totalRentPriceCart;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCart(string token)
        {
            var result = new ResultModel();
            try
            {
                var modelResponse = new CartShowModel();
                modelResponse.rentItems = new List<ItemRequest>();
                modelResponse.saleItems = new List<ItemRequest>();
                double? totalPrice = 0;
                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                var cart = await _cartRepo.GetCart(user.Id);
                if (cart == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                var listCartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                if (listCartDetail.Count == 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                foreach (var item in listCartDetail)
                {
                    if (item.IsForRent == true)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.SizeProductItemId);
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItemID = item.SizeProductItemId,
                            quantity = item.Quantity,
                            unitPrice = sizeProductItem.RentPrice
                        };
                        modelResponse.rentItems.Add(ItemRequest);
                        totalRentPriceCart += sizeProductItem.RentPrice * item.Quantity;
                    }
                    if (item.IsForRent == false)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.SizeProductItemId);
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItemID = item.SizeProductItemId,
                            quantity = item.Quantity,
                            unitPrice = sizeProductItem.SalePrice
                        };
                        modelResponse.saleItems.Add(ItemRequest);
                        totalSalePriceCart += sizeProductItem.SalePrice * item.Quantity;
                    }
                }
                modelResponse.totalRentPrice = totalRentPriceCart;
                modelResponse.totalSalePrice = totalSalePriceCart;
                modelResponse.totalPrice = totalSalePriceCart + totalRentPriceCart;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
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
