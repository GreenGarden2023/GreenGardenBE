using System;
using System.Data;
using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ProductModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("payment/")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IMoMoService _moMoService;
		public PaymentController(IMoMoService moMoService)
		{
            _moMoService = moMoService;
		}
        [HttpPost("deposit-payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> DepositPaymentMoMo(MoMoDepositModel moMoDepositModel)
        {
            Data.Models.ResultModel.ResultModel result = await _moMoService.DepositPaymentMoMo(moMoDepositModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("deposit-payment-cash")]
        [AllowAnonymous]
        public async Task<IActionResult> epositPaymentCash(MoMoDepositModel moMoDepositModel)
        {
            Data.Models.ResultModel.ResultModel result = await _moMoService.DepositPaymentCash(moMoDepositModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

