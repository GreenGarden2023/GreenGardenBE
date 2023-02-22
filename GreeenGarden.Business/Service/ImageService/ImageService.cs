using Azure.Storage.Blobs;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ImageService
{
    public class ImageService : IImageService
    {
        private readonly IImageRepo _imageRepo;

        public ImageService(IImageRepo imageRepo)
        {
            _imageRepo = imageRepo;
        }

        string defaultURL = "https://greengardenstorage.blob.core.windows.net/greengardensimages/";
        public async Task<ResultModel> UploadImage(IList<IFormFile> files)
        {
            ResultModel resultsModel = new ResultModel();
            List<string> urls = new List<string>();
            try
            {

                BlobContainerClient blobContainerClient = new BlobContainerClient(SecretService.SecretService.GetIMGConn(), "greengardensimages");
                foreach (IFormFile file in files)
                {
                    using (var stream = new MemoryStream())
                    {
                        Guid id = Guid.NewGuid();
                        string format = Path.GetExtension(file.FileName);
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                        urls.Add(defaultURL + id + format);
                    }
                }
                resultsModel.IsSuccess = true;
                resultsModel.Data = urls;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }

        }
        public async Task<ResultModel> UploadAnImage(IFormFile file)
        {
            ResultModel resultsModel = new ResultModel();
            string url = "";
            try
            {

                BlobContainerClient blobContainerClient = new BlobContainerClient(SecretService.SecretService.GetIMGConn(), "greengardensimages");

                using (var stream = new MemoryStream())
                {
                    Guid id = Guid.NewGuid();
                    string format = Path.GetExtension(file.FileName);
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                    url = defaultURL + id + format;
                }

                resultsModel.IsSuccess = true;
                resultsModel.Data = url;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }

        }
        public async Task<ResultModel> DeleteImages(List<string> fileURLs)
        {
            ResultModel resultsModel = new ResultModel();
            try
            {
                BlobContainerClient blobContainerClient = new BlobContainerClient(SecretService.SecretService.GetIMGConn(), "greengardensimages");
                foreach (string file in fileURLs)
                {
                    await blobContainerClient.DeleteBlobAsync(file.Replace(defaultURL, ""));
                }
                resultsModel.IsSuccess = true;
                resultsModel.Message = "Delete Files Successful";
                return resultsModel;
            }
            catch (Exception ex)
            {
                resultsModel.IsSuccess = false;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Delete Files Failed";
                return resultsModel;
            }
        }

        public async Task<ResultModel> UpdateImageCategory(Guid CategoryId, IFormFile file)
        {
            var result = new ResultModel();
            try
            {
                var imgBefore = await _imageRepo.GetImgUrlCategory(CategoryId);
                if (imgBefore != null)
                {
                    var imgToDelete = new List<string>() { imgBefore.ImageUrl };
                    await DeleteImages(imgToDelete);

                }

                var uploadImg = await UploadAnImage(file);
                if (uploadImg.IsSuccess)
                {

                    await _imageRepo.UpdateImgForCategory(CategoryId, uploadImg.Data.ToString());
                    result.IsSuccess = true;
                    result.Data = uploadImg.Data.ToString();
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }
        public async Task<ResultModel> UpdateImageProduct(Guid productID, IFormFile file)
        {
            var result = new ResultModel();
            try
            {
                var imgBefore = await _imageRepo.GetImgUrlProduct(productID);
                if (imgBefore != null)
                {
                    var imgToDelete = new List<string>() { imgBefore.ImageUrl };
                    await DeleteImages(imgToDelete);

                }

                var uploadImg = await UploadAnImage(file);
                if (uploadImg.IsSuccess)
                {

                    await _imageRepo.UpdateImgForProduct(productID, uploadImg.Data.ToString());
                    result.IsSuccess = true;
                    result.Data = uploadImg.Data.ToString();
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }
    }
}

