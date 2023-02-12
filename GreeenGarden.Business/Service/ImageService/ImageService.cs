using System;
using Azure.Storage.Blobs;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ImageService
{
	public class ImageService : IImageService
    {
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
    }
}

