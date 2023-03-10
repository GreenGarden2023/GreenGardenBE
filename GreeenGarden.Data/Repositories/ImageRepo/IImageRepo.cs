using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public interface IImageRepo : IRepository<TblImage>
    {
        Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl);
        Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl);
        Task<TblImage> UpdateImgForProductItem(Guid ProductItemID, string ImgUrl);
        Task<bool> UpdateImgForProductItemDetail(Guid SizeProductItemID, List<string> ImgUrls);
        Task<TblImage> GetImgUrlCategory(Guid categoryId);
        Task<TblImage> GetImgUrlProduct(Guid productId);
        Task<TblImage> GetImgUrlProductItem(Guid productItemID);
        Task<TblImage> GetImgUrlRentOrderDetail(Guid rentOrderDetailID);
        Task<TblImage> GetImgUrlSaleOrderDetail(Guid saleOrderDetailID);
        Task<List<string>> GetImgUrlProductItemDetail(Guid productitemDetailID);
        Task<bool> DeleteImage(string imgURL);
    }
}
