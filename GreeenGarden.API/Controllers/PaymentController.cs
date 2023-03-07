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

        [HttpPost("create-rent-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumPayment([Required] Guid addendumId, [Required] double amount)
        {
            try
            {
                var result = await _moMoService.CreateRentPayment(addendumId, amount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("create-rent-deposit-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumDepositPayment([Required] Guid addendumId)
        {
            try
            {
                var result = await _moMoService.CreateRentDepositPayment(addendumId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("receive-rent-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveAddendumPaymentResponse(MoMoResponseModel moMoResponseModel)
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
        [HttpPost("receive-rent-deposit-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveAddendumDepositPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessRentDepositPayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
    }
}

