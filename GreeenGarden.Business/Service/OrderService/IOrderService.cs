﻿using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.OrderService
{
	public interface IOrderService
	{
		Task<ResultModel> CreateRentOrder(string token, RentOrderModel rentOrderModel);
		Task<ResultModel> GetRentOrders(string token);
        Task<ResultModel> GetRentOrderDetail(string token, Guid rentOrderID);
    }
}

