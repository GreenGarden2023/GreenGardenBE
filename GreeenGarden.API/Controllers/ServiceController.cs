using System;
using GreeenGarden.Data.Models.ProductItemModel;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Business.Service.TakecareService;
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
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> CreateServiceRequest(ServiceInsertModel serviceInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.CreateRequest(token, serviceInsertModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-service-request-status")]
        [SwaggerOperation(Summary = "accepted/rejected")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> UpdateServiceRequestStatus(ServiceStatusModel serviceStatusModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.UpdateRequestStatus(token, serviceStatusModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-service-request")]
        [SwaggerOperation(Summary = "Get all service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetAllServiceRequest()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetAllRequest(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-user-service-request")]
        [SwaggerOperation(Summary = "Get current user's service request")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetUserServiceRequest()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareService.GetUserRequest(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

