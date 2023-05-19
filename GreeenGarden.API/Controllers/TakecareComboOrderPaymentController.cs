using System;
using System.Data;
using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreeenGarden.API.Controllers
{
    [Route("takecare-combo-order-payment")]
    [ApiController]
    public class TakecareComboOrderPaymentController : Controller
    {
        private readonly IMoMoService _moMoServices;
		public TakecareComboOrderPaymentController( IMoMoService moMoService)
		{
            _moMoServices = moMoService;
		}
        [HttpPost("deposit-payment")]
        [SwaggerOperation(Summary = "cash/momo")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderDepositPayment(TakecareComboOrderDepositPaymentModel takecareComboOrderDepositPaymentModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (takecareComboOrderDepositPaymentModel.PaymentMethod.Trim().ToLower().Equals("cash"))
            {
                Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderDepositPaymentCash(takecareComboOrderDepositPaymentModel.OrderId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            else
            {
                Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderDepositPaymentMoMo(takecareComboOrderDepositPaymentModel.OrderId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
        }

        [HttpPost("order-payment")]
        [SwaggerOperation(Summary = "Method: cash/momo ---- Type: whole/normal")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderPayment(TakecareComboOrderPaymentModel takecareComboOrderPaymentModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (takecareComboOrderPaymentModel.PaymentMethod.Trim().ToLower().Equals("momo"))
            {
                if (takecareComboOrderPaymentModel.PaymentType.Trim().ToLower().Equals("whole"))
                {
                    Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderWholePaymentMoMo(takecareComboOrderPaymentModel.OrderId);
                    return result.IsSuccess ? Ok(result) : BadRequest(result);
                }
                else
                {
                    Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderPaymentMoMo(takecareComboOrderPaymentModel.OrderId, (double)takecareComboOrderPaymentModel.Amount);
                    return result.IsSuccess ? Ok(result) : BadRequest(result);
                }
            }
            else
            {
                if (takecareComboOrderPaymentModel.PaymentType.Trim().ToLower().Equals("whole"))
                {
                    Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderWholePaymentCash(takecareComboOrderPaymentModel.OrderId);
                    return result.IsSuccess ? Ok(result) : BadRequest(result);
                }
                else
                {
                    Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderPaymentCash(takecareComboOrderPaymentModel.OrderId, (double)takecareComboOrderPaymentModel.Amount);
                    return result.IsSuccess ? Ok(result) : BadRequest(result);
                }
            }
        }

        [HttpPost("receive-deposit-payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveOrderDeposittMoMo(MoMoResponseModel moMoResponseModel)
        {
            _ = await _moMoServices.ProcessTakecareComboOrderDepositPaymentMoMo(moMoResponseModel);
            return NoContent();
        }

        [HttpPost("receive-order-payment-momo")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveOrderPaymentMoMo(MoMoResponseModel moMoResponseModel)
        {
            _ = await _moMoServices.ProcessTakecareComboOrderPaymentMoMo(moMoResponseModel);
            return NoContent();
        }
    }
}

