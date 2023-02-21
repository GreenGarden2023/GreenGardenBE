using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public interface IImageRepo : IRepository<TblImage>
    {
        Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl);
        Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl);
        Task<TblImage> GetImgUrl(Guid? imgId, Guid? categoryId, Guid? ProductItemID, Guid? productId, Guid? FeedbackID);
    }
}
