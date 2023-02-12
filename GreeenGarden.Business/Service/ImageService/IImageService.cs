using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ImageService
{
	public interface IImageService
	{
        public Task<ResultModel> UploadImage(IList<IFormFile> files);
        public Task<ResultModel> DeleteImages(List<string> fileURLs);
        public Task<string> InsertImages(TblImage model);
    }
}

