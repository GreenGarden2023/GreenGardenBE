using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public interface IImageRepo : IRepository<TblImage>
    {
        Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl);
        Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl);
        Task<TblImage> GetImgUrlCategory( Guid categoryId);
        Task<TblImage> GetImgUrlProduct(Guid productId);
    }
}
