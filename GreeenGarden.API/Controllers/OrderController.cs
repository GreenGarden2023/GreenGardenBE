﻿using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("order/")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService; 
		public OrderController(IOrderService orderService)
		{
            _orderService = orderService;
		}
        [HttpPost("create-rent-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> CreateRentOrder(RentOrderModel rentOrderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.CreateRentOrder(token, rentOrderModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetRentOrders()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrders(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-order-detail")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetRentOrdersDetail([Required] Guid rentOrderDetailID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrderDetail(token, rentOrderDetailID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

