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
        [HttpPost("upload-images-folder")]
        public async Task<ActionResult<ResultModel>> Upload([Required][FromForm] IList<IFormFile> files, [Required] string folderName)
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
                var result = await _imageService.UploadImagesFolder(files, folderName);
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
                var result = await _imageService.DeleteImagesByURLs(fileURLs);
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

    }
}

