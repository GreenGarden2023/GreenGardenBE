using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public interface IImageRepo : IRepository<TblImage>
    {
        Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl);
        Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl);
        Task<bool> UpdateImgForSizeProductItem(Guid SizeProductItemID, List<string> ImgUrls);
        Task<TblImage> GetImgUrlCategory( Guid categoryId);
        Task<TblImage> GetImgUrlProduct(Guid productId);
        Task<List<string>> GetImgUrlSizeProduct(Guid sizeproductItem);
        Task<bool> DeleteImage(string imgURL);
    }
}
