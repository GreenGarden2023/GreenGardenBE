﻿using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Business.Service.RevenueService;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.RevenueModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.API.Controllers
{
    [Route("revenue/")]
    //[Authorize]
    [ApiController]
    public class RevenueController : Controller
    {
        private readonly IRevenueService _service;
        public RevenueController(IRevenueService service)
        {
            _service = service;
        }
        [HttpGet("get-revenue-by-date-range")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRevenueByDateRange([FromQuery]RevenueReqByDateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.GetRevenueByDateRange(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-best-product-detail-by-date-range")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBestProductDetailByDateRange([FromQuery]RevenueReqByDateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.GetBestProductDetailByDateRange(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
