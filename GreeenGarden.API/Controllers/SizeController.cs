﻿using GreeenGarden.Business.Service.SizeService;
using GreeenGarden.Data.Models.SizeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("size/")]
    //[Authorize]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly ISizeService _service;
        public SizeController(ISizeService service)
        {
            _service = service;
        }

        [HttpPost("create-size")]
        [Authorize(Roles = "Staff, Manager")]
        public async Task<IActionResult> createSize([FromForm]SizeCreateModel sizeCreateModel)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.CreateSize(sizeCreateModel, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("get-sizes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSizes( )
        {
            var result = await _service.GetSizes();
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}