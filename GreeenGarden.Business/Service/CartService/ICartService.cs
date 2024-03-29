﻿using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.CartService
{
    public interface ICartService
    {
        //Task<ResultModel> GetCartShowModel(string token);
        Task<ResultModel> AddToCart(string token, AddToCartModel model);
        Task<ResultModel> GetCart(string token);
        Task<ResultModel> CleanCart(string token);
        Task<ResultModel> CleanSaleCart(string token);
        Task<ResultModel> CleanRentCart(string token);
    }
}
