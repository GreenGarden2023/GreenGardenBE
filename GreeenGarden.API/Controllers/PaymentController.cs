using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Repositories.AddendumRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.API.Controllers
{
    [Route("payment/")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMoMoService _moMoService;
        private readonly IAddendumRepo _addendumRepo;
        public PaymentController(IAddendumRepo addendumRepo, IMoMoService moMoService)
        {
            _moMoService = moMoService;
            _addendumRepo = addendumRepo;
        }

        [HttpPost("momo-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumPayment([Required] Guid addendumId, double? amount)
        {
            try
            {
                var addendum = await _addendumRepo.Get(addendumId);
                bool isRent;
                if (addendum != null && addendum.EndDateRent != null && addendum.StartDateRent != null)
                {
                    isRent = true;
                }
                else
                {
                    isRent = false;
                }
                if (isRent == true)
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
        [HttpPost("cash-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumPaymentCash([Required] Guid addendumId, double? amount)
        {
            try
            {
                var addendum = await _addendumRepo.Get(addendumId);
                bool isRent;
                if (addendum != null && addendum.EndDateRent != null && addendum.StartDateRent != null)
                {
                    isRent = true;
                }
                else
                {
                    isRent = false;
                }
                if (isRent == true)
                {
                    var result = await _moMoService.ProcessRentPaymentCash(addendumId, amount);
                    return Ok(result);
                }
                else
                {
                    var result = await _moMoService.ProcessSalePaymentCash(addendumId);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("momo-deposit-payment")]
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
        [HttpPost("cash-deposit-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAddendumCashDepositPayment([Required] Guid addendumId)
        {
            try
            {
                var result = await _moMoService.ProcessDepositPaymentCash(addendumId);

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
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
                return NoContent();
            }
        }
    }
}

