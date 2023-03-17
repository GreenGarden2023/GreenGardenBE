using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
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
        [HttpPost("create-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> CreateRentOrder(OrderCreateModel orderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (orderModel.StartDateRent != DateTime.MinValue &&  orderModel.EndDateRent != DateTime.MinValue )
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
        [HttpGet("get-rent-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetRentOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-rent-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetAllRentOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetAllRentOrders(token, pagingModel);
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
        [HttpGet("get-rent-order-group")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetRentOrdersGroup([Required] Guid groupID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrdersByGroup(token, groupID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-rent-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> UpdateRentOrderStatus(OrderUpdateModel orderUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.UpdateRentOrderStatus(token, orderUpdateModel.orderID, orderUpdateModel.status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetSaleOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-sale-orders")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetAllSaleOrders([FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetAllSaleOrders(token, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-order-detail")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetSaleOrdersDetail([Required] Guid saleOrderDetailID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrderDetail(token, saleOrderDetailID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-sale-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
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
    }
}

