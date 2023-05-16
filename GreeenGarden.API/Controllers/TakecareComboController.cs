using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.TakecareComboService;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.TakecareComboModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreeenGarden.API.Controllers
{
    [Route("takecare-combo/")]
    [ApiController]
    public class TakecareComboController : Controller
    {
        private readonly ITakecareComboService _takecareComboService;
        public TakecareComboController(ITakecareComboService takecareComboService)
        {
            _takecareComboService = takecareComboService;
        }
        [HttpPost("create-takecare-combo")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> CreateTakecareCombo([FromForm] TakecareComboInsertModel takecareComboInsertModel)
        {

            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboService.InsertTakecareCombo(takecareComboInsertModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-takecare-combo")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> UpdateTakecareCombo([FromForm] TakecareComboUpdateModel takecareComboUpdateModel)
        {

            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _takecareComboService.UpdateTakecareCombo(takecareComboUpdateModel, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-a-takecare-combo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTakecareCombo(Guid takecareComboID)
        {
            Data.Models.ResultModel.ResultModel result = await _takecareComboService.GetTakecareComboByID(takecareComboID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-all-takecare-combo")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "active/disabled/all")]
        public async Task<IActionResult> GetAllTakecareCombo(string status)
        {
            Data.Models.ResultModel.ResultModel result = await _takecareComboService.GetTakecareCombos(status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

