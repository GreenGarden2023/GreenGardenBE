using GreeenGarden.Business.Service.FeedbackService;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreeenGarden.API.Controllers
{
    [Route("feedback/")]
    //[Authorize]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }



        [HttpPost("create-feedback")]
        [Authorize]
        public async Task<IActionResult> createFeedback(FeedbackCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.createFeedback(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("update-feedback")]
        [Authorize]
        public async Task<IActionResult> updateFeedback(FeedbackUpdateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.updateFeedback(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("change-status")]
        [Authorize]
        public async Task<IActionResult> changeStatus(FeedbackChangeStatusModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _service.changeStatus(token, model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-list-feedback-by-product-item-detail")]
        public async Task<IActionResult> getListFeedback(Guid productItemDetailId)
        {
            Data.Models.ResultModel.ResultModel result = await _service.getListFeedbackByProductItem(productItemDetailId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-list-feedback-by-order")]
        public async Task<IActionResult> getListFeedbackByOrder( Guid orderID)
        {
            Data.Models.ResultModel.ResultModel result = await _service.getListFeedbackByOrder(orderID);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
