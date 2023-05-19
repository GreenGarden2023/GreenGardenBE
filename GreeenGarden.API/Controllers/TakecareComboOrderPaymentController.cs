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
        [HttpPost("deposit-payment-cash")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderDepositPaymentCash(TakecareComboOrderDepositPaymentModel takecareComboOrderDepositPaymentModel)
        {

                Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderDepositPaymentCash(takecareComboOrderDepositPaymentModel.OrderId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("deposit-payment-momo")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderDepositPaymentMoMo(TakecareComboOrderDepositPaymentModel takecareComboOrderDepositPaymentModel)
        {

                Data.Models.ResultModel.ResultModel result = await _moMoServices.TakecareComboOrderDepositPaymentMoMo(takecareComboOrderDepositPaymentModel.OrderId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("order-payment-cash")]
        [SwaggerOperation(Summary = "Type: whole/normal")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderPaymentCash(TakecareComboOrderPaymentModel takecareComboOrderPaymentModel)
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
        [HttpPost("order-payment-momo")]
        [SwaggerOperation(Summary = "Type: whole/normal")]
        [Authorize(Roles = "Technician, Manager, Customer")]
        public async Task<IActionResult> TakecareComboOrderPaymentMoMo(TakecareComboOrderPaymentModel takecareComboOrderPaymentModel)
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

