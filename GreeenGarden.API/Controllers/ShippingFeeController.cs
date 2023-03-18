using System;
using GreeenGarden.Business.Service.ShippingFeeService;
using GreeenGarden.Data.Models.ShippingFeeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("shipping-fee/")]
    [ApiController]
    public class ShippingFeeController : Controller
    {
        private readonly IShippingFeeService _shippingFeeService;
        public ShippingFeeController(IShippingFeeService shippingFeeService)
        {
            _shippingFeeService = shippingFeeService;
        }
        [HttpGet("get-list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListFee()
        {
            Data.Models.ResultModel.ResultModel result = await _shippingFeeService.GetListShipingFee();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-shipping-fee")]
        [Authorize(Roles = "Staff, Manager, Admin")]
        public async Task<IActionResult> UpdateShippingFee(List<ShippingFeeInsertModel> shippingFeeInsertModels)
        {
            Data.Models.ResultModel.ResultModel result = await _shippingFeeService.UpdateShippingFee(shippingFeeInsertModels);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}

