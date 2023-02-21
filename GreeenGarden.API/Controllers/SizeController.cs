﻿using GreeenGarden.Business.Service.SizeService;
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
        public async Task<IActionResult> createSize(string sizeName)
        {
            string token = (Request.Headers)["Authorization"].ToString().Split(" ")[1];
            var result = await _service.createSize(sizeName, token);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
