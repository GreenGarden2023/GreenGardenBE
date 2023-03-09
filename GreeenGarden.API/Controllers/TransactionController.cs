using GreeenGarden.Business.Service.TransactionService;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("transaction/")]
    //[Authorize]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _service;
        public TransactionController(ITransactionService service)
        {
            _service = service;
        }


        /// <summary>
        /// Create the first transaction, all transactions except the following use the API: pyByCashForPayment
        /// </summary>
        /// <returns></returns>
        /*[HttpPost("create-Payment")]
        public async Task<IActionResult> payByCashForAddendum([FromForm] TransactionRequestFirstModel model)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.payByCashForAddendum(token, model);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }*/
    }
}
