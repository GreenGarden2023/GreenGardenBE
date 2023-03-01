using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ICartRepo _cartRepo;
        public CartService( ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
            _decodeToken = new DecodeToken();
        }

        //public async Task<ResultModel> AddToCart(string token, AddToCartModel model)
        //{
        //    var result = new ResultModel();
        //    try
        //    {
        //        var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
        //        if (await _cartRepo.GetCart(user.Id) == null)
        //        {
        //            var cartTemp = new TblCart()
        //            {
        //                Id = Guid.NewGuid(),
        //                UserId = user.Id,
        //                Status = Status.ACTIVE,
        //                TotalPrice = 0,
        //                Quantity= 0,
        //            };
        //            await _cartRepo.Insert(cartTemp);
        //        }

        //        var productItem = await _cartRepo.GetProductItem(model.ProductItemId);
        //        var cart = await _cartRepo.GetCart(user.Id);

        //        if (productItem.Quantity < model.Quantity)
        //        {
        //            result.Code = 200;
        //            result.IsSuccess = false;
        //            result.Message = "Sản phẩm hiện đã hết trong kho!";
        //            return result;
        //        }

        //        var cartDetail = new TblCartDetail()
        //        {
        //            Id = Guid.NewGuid(),
        //            ProductItemId = model.ProductItemId,
        //            Quantity= model.Quantity,
        //            CartId= cart.Id,
        //        };
        //        await _cartRepo.AddProductItemToCart(cartDetail);
        //        cart.TotalPrice += (productItem.RentPrice * model.Quantity);
        //        cart.Quantity += model.Quantity;
        //        await _cartRepo.UpdateCart(cart);


        //        result.Code = 200;
        //        result.IsSuccess = true;
        //        result.Data = await _cartRepo.GetCartShow(user.Id);
        //    }
        //    catch (Exception e)
        //    {
        //        result.IsSuccess = false;
        //        result.Code = 400;
        //        result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
        //    }
        //    return result;

        //}

        //public async Task<ResultModel> GetCartShowModel(string token)
        //{
        //    var result = new ResultModel();
        //    try
        //    {
        //        var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
        //        var cart = await _cartRepo.GetCartShow(user.Id);
        //        if (cart == null)
        //        {
        //            result.Code = 200;
        //            result.IsSuccess = true;
        //            result.Data = "Cart's null";
        //        }

        //        result.Code = 200;
        //        result.IsSuccess = true;
        //        result.Data = cart;
        //    }
        //    catch (Exception e)
        //    {
        //        result.IsSuccess = false;
        //        result.Code = 400;
        //        result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
        //    }
        //    return result;

        //}
    }
}
