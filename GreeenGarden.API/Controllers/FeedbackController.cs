using GreeenGarden.Business.Service.FeedbackService;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("feedback/")]
    [Authorize]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }



        [HttpPost("create-feedback")]
        public async Task<IActionResult> createFeedback(FeedbackCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createFeedback(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("change-status")]
        public async Task<IActionResult> changeStatus(FeedbackChangeStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("get-list-feedback-by-product-item")]
        public async Task<IActionResult> getListFeedback([FromQuery] PaginationRequestModel pagingModel, Guid productItemID)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.getListFeedbackByProductItem(token, pagingModel, productItemID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
