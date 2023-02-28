using System.Data;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Data.Models.FileModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("upload")]
        public async Task<ActionResult<ResultModel>> Upload(IList<IFormFile> files)
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
                var result = await _imageService.UploadImages(files);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }
                return Ok(result);
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
                var result = await _imageService.DeleteImages(fileURLs);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }
                return Ok(result);
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
        [HttpGet("get-an-image")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFile(string fileUrl)
        {
            try
            {
                FileData fileData = await _imageService.DownloadAnImage(fileUrl);
                return File(fileData.bytes, fileData.contenType, fileData.name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

