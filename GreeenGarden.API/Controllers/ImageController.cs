using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreeenGarden.API.Controllers
{
    [Route("image/")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpPost("upload-images")]
        public async Task<ActionResult<ResultModel>> Upload([Required][FromForm] List<IFormFile> files)
        {
            if (!files.Any())
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Message = "Files Empty"

                });
            };
            try
            {
                ResultModel result = await _imageService.UploadImages(files);
                return result.IsSuccess == false ? (ActionResult<ResultModel>)BadRequest(result) : (ActionResult<ResultModel>)Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Data = ex.ToString(),
                    Message = "Upload Failed"

                });
            }
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<ResultModel>> Delete(List<string> fileURLs)
        {
            if (!fileURLs.Any())
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Message = "Url Empty"

                });
            };
            try
            {
                ResultModel result = await _imageService.DeleteImages(fileURLs);
                return result.IsSuccess == false ? (ActionResult<ResultModel>)BadRequest(result) : (ActionResult<ResultModel>)Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Data = ex.ToString(),
                    Message = "Upload Failed"
                });
            }
        }
        [HttpDelete("delete-by-urls")]
        public async Task<ActionResult<ResultModel>> DeleteByURLs(List<string> fileURLs)
        {
            if (!fileURLs.Any())
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Message = "Url Empty"

                });
            };
            try
            {
                ResultModel result = await _imageService.DeleteImagesByURLs(fileURLs);
                return result.IsSuccess == false ? (ActionResult<ResultModel>)BadRequest(result) : (ActionResult<ResultModel>)Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Data = ex.ToString(),
                    Message = "Upload Failed"
                });
            }
        }

        [HttpPost("re-upload-image")]
        public async Task<ActionResult<ResultModel>> ReUpload(string oldImageURL)
        {

            try
            {
                ResultModel result = await _imageService.ReUpload(oldImageURL);
                return result.IsSuccess == false ? (ActionResult<ResultModel>)BadRequest(result) : (ActionResult<ResultModel>)Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel()
                {
                    IsSuccess = false,
                    Data = ex.ToString(),
                    Message = "Upload Failed"

                });
            }
        }
    }
}

