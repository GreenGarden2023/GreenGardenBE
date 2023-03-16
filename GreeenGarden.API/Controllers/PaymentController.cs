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
        [HttpPost("receive-deposit-payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveDepositPaymentMoMo(MoMoResponseModel moMoResponseModel)
        {
             _ = await _moMoService.ProcessDepositPaymentMoMo(moMoResponseModel);
            return  NoContent();
        }
        [HttpPost("receive-order-payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveOrderPaymentMoMo(MoMoResponseModel moMoResponseModel)
        {
            _ = await _moMoService.ProcessOrderPaymentMoMo(moMoResponseModel);
            return NoContent();
        }
        [HttpPost("deposit-payment-cash")]
        [AllowAnonymous]
        public async Task<IActionResult> DepositPaymentCash(MoMoDepositModel moMoDepositModel)
        {
            Data.Models.ResultModel.ResultModel result = await _moMoService.DepositPaymentCash(moMoDepositModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("payment-cash")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCash(MoMoPaymentModel moMoPaymentModel)
        {
            if (moMoPaymentModel.PaymentType.Trim().ToLower().Equals("whole"))
            {
                Data.Models.ResultModel.ResultModel result = await _moMoService.OrderPaymentCash(moMoPaymentModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                Data.Models.ResultModel.ResultModel result = await _moMoService.WholeOrderPaymentCash(moMoPaymentModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
        }
        [HttpPost("payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentMoMo(MoMoPaymentModel moMoPaymentModel)
        {
            if (moMoPaymentModel.PaymentType.Trim().ToLower().Equals("whole"))
            {
                Data.Models.ResultModel.ResultModel result = await _moMoService.WholeOrderPaymentMoMo(moMoPaymentModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                Data.Models.ResultModel.ResultModel result = await _moMoService.OrderPaymentMoMo(moMoPaymentModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
        }

    }
}

