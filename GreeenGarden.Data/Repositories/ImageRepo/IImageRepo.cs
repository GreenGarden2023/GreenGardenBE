using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ImageRepo
{
    public interface IImageRepo : IRepository<TblImage>
    {
        Task<TblImage> UpdateImgForCategory(Guid categoryId, string imgUrl);
        Task<TblImage> UpdateImgForProduct(Guid ProductID, string ImgUrl);
        Task<TblImage> UpdateImgForProductItem(Guid ProductItemID, string ImgUrl);
        Task<bool> UpdateImgForProductItemDetail(Guid SizeProductItemID, List<string> ImgUrls);
        Task<bool> UpdateImgForUserTree(Guid userTreeID, List<string> ImgUrls);
        Task<TblImage> GetImgUrlCategory(Guid categoryId);
        Task<TblImage> GetImgUrlProduct(Guid productId);
        Task<TblImage> GetImgUrlProductItem(Guid productItemID);
        Task<TblImage> GetImgUrlRentOrderDetail(Guid rentOrderDetailID);
        Task<TblImage> GetImgUrlSaleOrderDetail(Guid saleOrderDetailID);
        Task<List<string>> GetImgUrlProductItemDetail(Guid productitemDetailID);
        Task<List<string>> GetImgUrlUserTree(Guid userTreeID);
        Task<List<string>> GetImgUrlServiceDetail(Guid serviceDetailId);
        Task<List<string>> GetImgUrlFeedback(Guid feedbackID);
        Task<List<string>> GetImgUrlServiceCalendar(Guid serviceCalendarID);
        Task<List<string>> GetImgUrlComboServiceCalendar(Guid serviceCalendarID);
        Task<bool> UpdateImgForServiceCalendar(Guid serviceCalendarID, List<string> ImgUrls);
        Task<bool> UpdateImgForComboServiceCalendar(Guid serviceCalendarID, List<string> ImgUrls);
        Task<bool> DeleteImage(string imgURL);
    }
}
