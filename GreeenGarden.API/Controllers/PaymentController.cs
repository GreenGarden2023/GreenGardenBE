using System;
using GreeenGarden.Business.Service.PaymentService;
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
        public async Task<IActionResult> CreatePayment()
        {
            try
            {
                var result = await _moMoService.CreatePayment();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}

