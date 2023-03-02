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

        [HttpPost("create-order-payment")]
        public async Task<IActionResult> CreateOrderPayment([Required] Guid orderID)
        {
            try
            {
                var result = await _moMoService.CreateOrderPayment(orderID);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        [HttpPost("create-addendum-payment")]
        public async Task<IActionResult> CreateAddendumPayment([Required] Guid addendumId, [Required] double amount)
        {
            try
            {
                var result = await _moMoService.CreateAddendumPayment(addendumId, amount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        [HttpPost("receive-order-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveOrderPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessOrderPayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
        [HttpPost("receive-addendum-payment-reponse")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveAddendumPaymentResponse(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var result = await _moMoService.ProcessAddendumPayment(moMoResponseModel);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
    }
}

