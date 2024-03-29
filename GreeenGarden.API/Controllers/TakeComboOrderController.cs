﻿using System;
using System.Data;
using GreeenGarden.Business.Service.TakecareComboOrderService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("takecare-combo-order/")]
    [ApiController]
    public class TakeComboOrderController : Controller
    {
        private readonly ITakecareComboOrderService _takecareComboOrderService;
		public TakeComboOrderController(ITakecareComboOrderService takecareComboOrderService)
		{
            _takecareComboOrderService = takecareComboOrderService;
		}
        [HttpPost("create-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CreateOrder(TakecareComboOrderCreateModel orderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.CreateTakecareComboOrder(orderModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-order-by-id")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetOrderByID(Guid orderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetTakecareComboOrderByID(orderID, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-order-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetTakecareComboOrderByOrderCode(string orderCode, string phone)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetTakecareComboOrderByOrderCode(orderCode, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-order-by-phone")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetTakecareComboOrderByPhone([FromQuery]TakecareComboOrderSearchPhoneModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetTakecareComboOrderByPhone(model, pagingModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAllOrder([FromQuery] PaginationRequestModel pagingModel, string status)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetAllTakcareComboOrder(pagingModel,status, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-orders-by-technician")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAllOrderByTechnician([FromQuery] PaginationRequestModel pagingModel, [FromQuery] TakecareComboOrderTechnicianReqModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetAllTakcareComboOrderForTechnician(pagingModel, model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-combo-order-detail-by-range-date")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrderDetailByRangeDate([FromQuery] OrderRangeDateReqModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.GetServiceOrderDetailByRangeDate(model, pagingModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateOrderStatus(TakecareComboOrderUpdateStatusModel orderUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.ChangeTakecareComboOrderStatus(orderUpdateModel.TakecareComboOrderId, orderUpdateModel.Status, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cancel-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CancelOrder(TakecareComboOrderCancelModel takecareComboOrderCancelModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _takecareComboOrderService.CancelTakecareComboOrder(takecareComboOrderCancelModel.TakecareComboOrderId, takecareComboOrderCancelModel.CancelReason, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

