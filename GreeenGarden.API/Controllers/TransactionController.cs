
using System;
using GreeenGarden.Business.Service.TransactionService;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.TransactionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreeenGarden.API.Controllers
{
    [Route("transaction/")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
		public TransactionController(ITransactionService transactionService)
		{
            _transactionService = transactionService;
		}
        [HttpGet("get-transaction-by-order")]
        [SwaggerOperation(Summary = "rent/sale/service")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetTransactionByOrder([FromQuery] TransactionGetByOrderModel transactionGetByOrderModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _transactionService.GetTransactionByOrder(token, transactionGetByOrderModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-transaction-by-date")]
        [SwaggerOperation(Summary = "mm/dd/yyyy")]
        [Authorize(Roles = "Staff, Manager, Admin, Customer")]
        public async Task<IActionResult> GetTransactionByDate([FromQuery] TransactionGetByDateModel transactionGetByDateModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _transactionService.GetTransactionByDate(token, transactionGetByDateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}