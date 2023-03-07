using System.ComponentModel.DataAnnotations;
using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Data.Models.MoMoModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("payment/")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMoMoService _moMoService;
        public PaymentController(IMoMoService moMoService)
        {
            _moMoService = moMoService;
        }

        [HttpPost("create-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumPayment([Required] Guid addendumId, double? amount, [Required] bool isRent)
        {
            try
            {
                if(isRent == true)
                {
                    var result = await _moMoService.CreateRentPayment(addendumId, amount);
                    return Ok(result);
                }
                else
                {
                    var result = await _moMoService.CreateSalePayment(addendumId);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("create-deposit-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumDepositPayment([Required] Guid addendumId)
        {
            try
            {
                var result = await _moMoService.CreateDepositPayment(addendumId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("receive-rent-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveRenyAddendumPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessRentPayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
        [HttpPost("receive-sale-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveSaleAddendumPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessSalePayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
        [HttpPost("receive-deposit-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveAddendumDepositPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessDepositPayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
    }
}

