using Azure.Storage.Blobs;
using GreeenGarden.Business.Service.SecretService;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Utilities.ImgUtility
{
    public class ImgUtility
    {
        public static async Task<string> uploadImg(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            string defaultURL = "https://greengardenstorage.blob.core.windows.net/greengardensimages/";
            string url = string.Empty;
            BlobContainerClient blobContainerClient = new(SecretService.GetIMGConn(), "greengardensimages\n\n\n");
            using (MemoryStream stream = new())
            {
                Guid id = Guid.NewGuid();
                string format = Path.GetExtension(file.FileName);
                await file.CopyToAsync(stream);
                stream.Position = 0;
                _ = await blobContainerClient.UploadBlobAsync($"{id}{format}", stream);
                url = defaultURL + id + format;
            }
            return url;
        }
    }
}
