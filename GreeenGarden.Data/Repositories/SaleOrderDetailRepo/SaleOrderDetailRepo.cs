using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.FeedbackRepo;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SaleOrderDetailRepo
{
    public class SaleOrderDetailRepo : Repository<TblSaleOrderDetail>, ISaleOrderDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IImageRepo _imageRepo;
        private readonly IFeedbackRepo _feedbackRepo;
        public SaleOrderDetailRepo(GreenGardenDbContext context, IFeedbackRepo feedbackRepo, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _imageRepo = imageRepo;
            _feedbackRepo = feedbackRepo;   
        }

        public async Task<List<SaleOrderDetailResModel>> GetSaleOrderDetails(Guid saleOrderId)
        {
            List<TblSaleOrderDetail> list = await _context.TblSaleOrderDetails.Where(x => x.SaleOderId.Equals(saleOrderId)).ToListAsync();
            List<SaleOrderDetailResModel> resultList = new();
            foreach (TblSaleOrderDetail detail in list)
            {
                TblImage image = await _imageRepo.GetImgUrlSaleOrderDetail(detail.Id);
                string imageURl = "";
                if (image != null)
                {
                    imageURl = image.ImageUrl;
                }
                List<FeedbackOrderResModel> fbList = await _feedbackRepo.GetFeedBackOrderDetail(saleOrderId, (Guid)detail.ProductItemDetailId);
                var itemDetail = await _context.TblProductItemDetails.Where(x => x.Id.Equals(detail.ProductItemDetailId)).FirstOrDefaultAsync();
                var productItem = await _context.TblProductItems.Where(x=>x.Id.Equals(itemDetail.ProductItemId)).FirstOrDefaultAsync();
                SaleOrderDetailResModel model = new()
                {
                    ID = detail.Id,
                    Quantity = detail.Quantity ?? null,
                    TotalPrice = detail.TotalPrice ?? null,
                    SalePricePerUnit = detail.SalePricePerUnit ?? null,
                    SizeName = "" + detail.SizeName,
                    ProductItemName = "" + detail.ProductItemName,
                    CareGuide = productItem.CareGuide,
                    ProductItemDetailID = detail.ProductItemDetailId,
                    ImgURL = imageURl,
                    FeedbackList = fbList,
                };
                resultList.Add(model);
            }
            return resultList;
        }

        public async Task<bool> UpdateSaleOrderDetails(TblSaleOrderDetail entity)
        {
            _ = _context.TblSaleOrderDetails.Update(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }
    }
}

