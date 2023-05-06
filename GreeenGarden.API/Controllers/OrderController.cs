using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Models.FileModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.API.Controllers
{
    [Route("order/")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IEMailService _emailService;
        public OrderController(IOrderService orderService, IEMailService eMailService)
        {
            _orderService = orderService;
            _emailService = eMailService;
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
        [HttpGet("get-rent-order-group-by-order")]
        public async Task<IActionResult> GetRentOrderGroupByOrderID(Guid orderID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrderGroupByOrderID(token, orderID);
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
        [HttpGet("get-service-orders-by-technician-today")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrderByTechnicianToday([FromQuery] PaginationRequestModel pagingModel, string? takecareStatus, [Required] Guid technicianID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderByTechnicianToday(token, pagingModel, technicianID, takecareStatus);
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
                ResultModel result = await _orderService.CancelSaleOrderById(token, orderCancelModel.orderID, orderCancelModel.reason);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else if (orderCancelModel.orderType.Trim().ToLower().Equals("rent"))
            {
                ResultModel result = await _orderService.CancelRentOrderById(token, orderCancelModel.orderID, orderCancelModel.reason);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else if (orderCancelModel.orderType.Trim().ToLower().Equals("service"))
            {
                ResultModel result = await _orderService.CancelServiceOrderById(token, orderCancelModel.orderID, orderCancelModel.reason);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                return BadRequest("Order type unknown.");
            }
        }
        [HttpGet("get-rent-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrderDetailByOrderCode([FromQuery] OrderFilterModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.SearchRentOrderDetail(token, model, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-sale-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetSaleOrderDetailByOrderCode([FromQuery] OrderFilterModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetSaleOrderDetailByOrderCode(token, model, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-order-detail-by-order-code")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrderDetailByOrderCode([FromQuery] OrderFilterModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderDetailByOrderCode(token, model, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-rent-order-detail-by-range-date")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRentOrderDetailByRangeDate([FromQuery]OrderRangeDateReqModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetRentOrderDetailByRangeDate(token, model, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-service-order-detail-by-range-date")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetServiceOrderDetailByRangeDate([FromQuery]OrderRangeDateReqModel model, [FromQuery] PaginationRequestModel pagingModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.GetServiceOrderDetailByRangeDate(token, model, pagingModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-end-date-takecare-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateDateTakecare(UpdateDateTakecareModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.UpdateDateTakecare(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-service-order-status")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateServiceOrderStatus(UpdateServiceOrderStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _orderService.UpdateServiceOrderStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("generate-pdf")]
        [AllowAnonymous]
        public async Task<IActionResult> GeneratePDF(Guid orderID)
        {
            ResultModel result = await _orderService.GeneratePDF(orderID);
            FileData file = (FileData)result.Data;
            return result.IsSuccess ? File(file.bytes, file.contenType, file.name) : BadRequest(result);
        }
        [HttpPost("send-contract-email")]
        [AllowAnonymous]
        public async Task<IActionResult> SendContractEmail(string email, Guid orderID)
        {
            ResultModel resultGen = await _orderService.GeneratePDF(orderID);
            FileData file = (FileData)resultGen.Data;
            ResultModel result = await _emailService.SendEmailRentOrderContract(email ,orderID, file);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
