using System.Collections;
using Azure.Storage.Blobs;
using GreeenGarden.Data.Models.FileModel;
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

        private readonly string defaultURL = "https://greengardenstorage.blob.core.windows.net/greengardensimages/";
        public async Task<ResultModel> UploadImages(IList<IFormFile> files)
        {
            ResultModel resultsModel = new();
            List<string> urls = new();
            try
            {

                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");
                foreach (IFormFile file in files)
                {
                    using MemoryStream stream = new();
                    Guid id = Guid.NewGuid();
                    string format = Path.GetExtension(file.FileName);
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    _ = await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                    urls.Add(defaultURL + id + format);
                }
                resultsModel.IsSuccess = true;
                resultsModel.Code = 200;
                resultsModel.Data = urls;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Code = 400;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }

        }

        public async Task<ResultModel> UploadAnImage(IFormFile file)
        {
            ResultModel resultsModel = new();
            string url = "";
            try
            {

                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");

                using (MemoryStream stream = new())
                {
                    Guid id = Guid.NewGuid();
                    string format = Path.GetExtension(file.FileName);
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    _ = await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                    url = defaultURL + id + format;
                }

                resultsModel.IsSuccess = true;
                resultsModel.Code = 200;
                resultsModel.Data = url;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Code = 400;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }

        }

        public async Task<ResultModel> UploadAFile(IFormFile file)
        {
            ResultModel resultsModel = new();
            string url = "";
            try
            {

                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");

                using (MemoryStream stream = new())
                {
                    Guid id = Guid.NewGuid();
                    string format = Path.GetExtension(file.FileName);
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    _ = await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                    url = defaultURL + id + format;
                }

                resultsModel.IsSuccess = true;
                resultsModel.Code = 200;
                resultsModel.Data = url;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Code = 400;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }

        }
        public async Task<ResultModel> DeleteImages(List<string> fileURLs)
        {
            ResultModel resultsModel = new();
            try
            {
                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");
                foreach (string file in fileURLs)
                {
                    _ = await blobContainerClient.DeleteBlobAsync(file.Replace(defaultURL, ""));
                    _ = await _imageRepo.DeleteImage(file);
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
            ResultModel result = new();
            try
            {
                Data.Entities.TblImage imgBefore = await _imageRepo.GetImgUrlCategory(CategoryId);
                if (imgBefore != null)
                {
                    List<string> imgToDelete = new() { imgBefore.ImageUrl };
                    _ = await DeleteImages(imgToDelete);
                }

                ResultModel uploadImg = await UploadAnImage(file);
                if (uploadImg.IsSuccess)
                {
                    _ = await _imageRepo.UpdateImgForCategory(CategoryId, uploadImg.Data.ToString());
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
            ResultModel result = new();
            try
            {
                Data.Entities.TblImage imgBefore = await _imageRepo.GetImgUrlProduct(productID);
                if (imgBefore != null)
                {
                    List<string> imgToDelete = new() { imgBefore.ImageUrl };
                    _ = await DeleteImages(imgToDelete);

                }

                ResultModel uploadImg = await UploadAnImage(file);
                if (uploadImg.Code == 200)
                {

                    Data.Entities.TblImage updateImg = await _imageRepo.UpdateImgForProduct(productID, uploadImg.Data.ToString());
                    if (updateImg != null)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Data = uploadImg.Data.ToString();
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Data = "Update product image failed.";
                    }
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

        public async Task<ResultModel> DeleteImagesByURLs(List<string> fileURLs)
        {
            ResultModel resultsModel = new();
            try
            {
                foreach (string file in fileURLs)
                {
                    BlobClient blobClient = new(new Uri(file), new Azure.Storage.StorageSharedKeyCredential("greengardenstorage", SecretService.SecretService.GetStorageKey()));
                    _ = await blobClient.DeleteAsync();
                }
                resultsModel.IsSuccess = true;
                resultsModel.Code = 200;
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

        public async Task<string> ReUpload(string oldFileURL)
        {
            try
            {
                string sourceBlobName = oldFileURL.Replace(defaultURL, "");

                string sourceExtension = Path.GetExtension(sourceBlobName);

                string destinationBlobName = Guid.NewGuid().ToString() + sourceExtension;

                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");
                BlobClient sourceBlobClient = blobContainerClient.GetBlobClient(sourceBlobName);

                MemoryStream memoryStream = new();
                _ = await sourceBlobClient.DownloadToAsync(memoryStream);

                _ = memoryStream.Seek(0, SeekOrigin.Begin);

                BlobClient destinationBlobClient = blobContainerClient.GetBlobClient(destinationBlobName);
                _ = await destinationBlobClient.UploadAsync(memoryStream);

                // Close the memory stream
                memoryStream.Close();
                return defaultURL + destinationBlobName;
            }
            catch
            {
                return "";
            }
        }

        public async Task<ResultModel> UploadAPDF(FileData file)
        {
            ResultModel resultsModel = new();
            string url = "";
            try
            {

                BlobContainerClient blobContainerClient = new(SecretService.SecretService.GetIMGConn(), "greengardensimages");

                using (MemoryStream stream = new())
                {
                    Guid id = Guid.NewGuid();
                    string format = Path.GetExtension(file.name);
                    Stream streamArr = new MemoryStream(file.bytes);
                    _ = await blobContainerClient.UploadBlobAsync($"{id}{format}", streamArr);
                    url = defaultURL + id + format;
                }

                resultsModel.IsSuccess = true;
                resultsModel.Code = 200;
                resultsModel.Data = url;
                resultsModel.Message = "Upload Success";


                return resultsModel;
            }
            catch (Exception ex)
            {

                resultsModel.IsSuccess = false;
                resultsModel.Code = 400;
                resultsModel.Data = ex.ToString();
                resultsModel.Message = "Upload Failed";
                return resultsModel;
            }
        }
    }
}


