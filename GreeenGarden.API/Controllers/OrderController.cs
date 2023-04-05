﻿using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

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
        [HttpPost("create-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CreateOrder(OrderCreateModel orderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (orderModel.StartDateRent != DateTime.MinValue && orderModel.EndDateRent != DateTime.MinValue)
            {
                ResultModel result = await _orderService.CreateRentOrder(token, orderModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                ResultModel result = await _orderService.CreateSaleOrder(token, orderModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
        }
        [HttpPost("create-service-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CreateServiceOrder(ServiceOrderCreateModel orderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.CreateServiceOrder(token, orderModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-rent-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAllRentOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetAllRentOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-order-detail")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrdersDetail([Required] Guid rentOrderDetailID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrderDetail(token, rentOrderDetailID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-order-group")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrdersGroup([Required] Guid groupID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrdersByGroup(token, groupID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-a-rent-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetARentOrdersDetail([Required] Guid rentOrderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetARentOrder(token, rentOrderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-rent-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateRentOrderStatus(OrderUpdateModel orderUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.UpdateRentOrderStatus(token, orderUpdateModel.orderID, orderUpdateModel.status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetSaleOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-sale-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAllSaleOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetAllSaleOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-order-detail")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetSaleOrdersDetail([Required] Guid saleOrderDetailID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrderDetail(token, saleOrderDetailID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-orders-by-technician")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrdersByTechnician([FromQuery] PaginationRequestModel pagingModel, [Required] Guid technicianID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderByTechnician(token, pagingModel, technicianID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-service-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAllServiceOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetAllServiceOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-a-service-order-detail")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAServiceOrderDetail(Guid orderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderById(token, orderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-sale-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateSaleOrderStatus(OrderUpdateModel orderUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.UpdateSaleOrderStatus(token, orderUpdateModel.orderID, orderUpdateModel.status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("calculate-order")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculateRentOrder(OrderCreateModel orderModel)
        {
            if (orderModel.StartDateRent != DateTime.MinValue && orderModel.EndDateRent != DateTime.MinValue)
            {
                ResultModel result = await _orderService.CalculateRentOrder(orderModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                ResultModel result = await _orderService.CalculateSaleOrder(orderModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
        }
        [HttpPost("cancel-order")]
        [SwaggerOperation(Summary = "rent/sale/service")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CancelServiceOrderById(OrderCancelModel orderCancelModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (orderCancelModel.orderType.Trim().ToLower().Equals("sale"))
            {
                ResultModel result = await _orderService.CancelSaleOrderById(token, orderCancelModel.orderID);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else if (orderCancelModel.orderType.Trim().ToLower().Equals("rent"))
            {
                ResultModel result = await _orderService.CancelRentOrderById(token, orderCancelModel.orderID);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else if (orderCancelModel.orderType.Trim().ToLower().Equals("service"))
            {
                ResultModel result = await _orderService.CancelServiceOrderById(token, orderCancelModel.orderID);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                return BadRequest("Order type unknown.");
            }
        }

        [HttpGet("get-rent-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrderDetailByOrderCode(string orderCode)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrderDetailByOrderCode(token, orderCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetSaleOrderDetailByOrderCode(string orderCode)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrderDetailByOrderCode(token, orderCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrderDetailByOrderCode(string orderCode)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderDetailByOrderCode(token, orderCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
