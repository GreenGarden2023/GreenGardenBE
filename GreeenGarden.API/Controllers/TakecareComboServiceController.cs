using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GreeenGarden.Business.Service.ShippingFeeService;
using GreeenGarden.Business.Service.TakecareComboServiceServ;
using GreeenGarden.Data.Models.ShippingFeeModel;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreeenGarden.API.Controllers
{
    [Route("takecare-combo-service")]
    [ApiController]
    public class TakecareComboServiceController : Controller
    {
        private readonly ITakecareComboServiceServ _takecareComboServiceServ;
        public TakecareComboServiceController(ITakecareComboServiceServ takecareComboServiceServ)
        {
            _takecareComboServiceServ = takecareComboServiceServ;
        }
        [HttpPost("create-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> CreateTakecareComboService(TakecareComboServiceInsertModel takecareComboServiceInsertModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.CreateTakecareComboService(takecareComboServiceInsertModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-a-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> GetATakecareComboService(Guid takecareComboServiceID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.GetTakecareComboServiceByID(takecareComboServiceID, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-takecare-combo-service-by-code")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> GetTakecareComboServiceByCode(string code)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.GetTakecareComboServiceByCode(code, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        [SwaggerOperation(Summary = "pending/accepted/rejected/all")]
        public async Task<IActionResult> GetAllTakecareComboService(string status)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.GetAllTakecareComboService(status, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-all-takecare-combo-service-by-tech")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        [SwaggerOperation(Summary = "pending/accepted/rejected/all")]
        public async Task<IActionResult> GetAllTakecareComboServiceForTechnician(string status, Guid technicianId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.GetAllTakecareComboServiceForTechnician(status, token, technicianId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("change-takecare-combo-service-status")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        [SwaggerOperation(Summary = "pending/accepted/rejected")]
        public async Task<IActionResult> ChangeTakecareComboServiceStatus(TakecareComboServiceChangeStatusModel takecareComboServiceChangeStatusModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.ChangeTakecareComboServiceStatus(takecareComboServiceChangeStatusModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("assign-takecare-combo-service-technician")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> AssignTakecareComboService(TakecareComboServiceAssignTechModel takecareComboServiceAssignTechModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.AssignTechnicianTakecareComboService(takecareComboServiceAssignTechModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.UpdateTakecareComboService(takecareComboServiceUpdateModel,token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("cancel-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> CancelTakecareComboService(TakecareComboServiceCancelModel takecareComboServiceCancelModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.CancelService(takecareComboServiceCancelModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("reject-takecare-combo-service")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> RejectService(TakecareComboServiceRejectModel takecareComboServiceRejectModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboServiceServ.RejectService(takecareComboServiceRejectModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

