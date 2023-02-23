using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ImageService
{
    public interface IImageService
    {
        public Task<ResultModel> UploadImages(IList<IFormFile> files);
        public Task<ResultModel> DeleteImages(List<string> fileURLs);
        public Task<ResultModel> UploadAnImage(IFormFile file);
        public Task<ResultModel> UpdateImageCategory(Guid CategoryId, IFormFile file);
        public Task<ResultModel> UpdateImageProduct(Guid ProductID, IFormFile file);
    }
}

