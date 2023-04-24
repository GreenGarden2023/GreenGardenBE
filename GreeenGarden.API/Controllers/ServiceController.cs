using GreeenGarden.Business.Service.TakecareService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreeenGarden.API.Controllers
{
    [Route("service/")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly ITakecareService _takecareService;
        public ServiceController(ITakecareService takecareService)
        {
            _takecareService = takecareService;
        }
        [HttpPost("create-service-request")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CreateServiceRequest(ServiceInsertModel serviceInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.CreateRequest(token, serviceInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-service-request-status")]
        [SwaggerOperation(Summary = "accepted/rejected")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> UpdateServiceRequestStatus(ServiceStatusModel serviceStatusModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.UpdateRequestStatus(token, serviceStatusModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-service-request")]
        [SwaggerOperation(Summary = "Get all service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Technician")]
        public async Task<IActionResult> GetAllServiceRequest()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetAllRequest(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-request-order-by-technician")]
        [SwaggerOperation(Summary = "Get all service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Technician")]
        public async Task<IActionResult> GetRequestOrderByTechnician([FromQuery]PaginationRequestModel pagingModel, Guid technicianID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetRequestOrderByTechnician(token, pagingModel, technicianID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-user-service-request")]
        [SwaggerOperation(Summary = "Get current user's service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetUserServiceRequest()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetUserRequest(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-a-service-request-detail")]
        [SwaggerOperation(Summary = "Get current user's service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetAServiceRequest(Guid serviceRequestID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetARequestDetail(token, serviceRequestID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("assign-service-technician")]
        [SwaggerOperation(Summary = "Assign a technician to a service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Technician")]
        public async Task<IActionResult> AssignTechnician(ServiceAssignModelManager serviceAssignModelManager)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.AssignTechnician(token, serviceAssignModelManager);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-service-detail")]
        [SwaggerOperation(Summary = "Update service and service detail for manager")]
        [Authorize(Roles = "Staff, Manager, Admin, Technician")]
        public async Task<IActionResult> UpdateServiceDetail(UpdateService updateService)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.UpdateServicePrice(token, updateService.ServiceUpdate, updateService.ServiceDetailUpdate);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-request-detail-by-service-order")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> GetRequestDetailByServiceOrder(string serviceOrder)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetRequestDetailByServiceOrder(token, serviceOrder);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cancel-request")]
        [Authorize(Roles = "Manager, Admin, Customer, Technician")]
        public async Task<IActionResult> CancelRequest(CancelRequestModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.CancelRequest(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

