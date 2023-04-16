using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.FeedbackRepo;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RentOrderDetailRepo
{
    public class RentOrderDetailRepo : Repository<TblRentOrderDetail>, IRentOrderDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        private readonly IProductItemRepo _productItemRepo;
        private readonly IImageRepo _imageRepo;
        private readonly ISizeRepo _sizeRepo;
        private readonly IFeedbackRepo _feedbackRepo;
        public RentOrderDetailRepo(GreenGardenDbContext context, IFeedbackRepo feedbackRepo, ISizeRepo sizeRepo, IProductItemDetailRepo sizeProductItemRepo, IImageRepo imageRepo, IProductItemRepo productItemRepo) : base(context)
        {
            _context = context;
            _productItemDetailRepo = sizeProductItemRepo;
            _imageRepo = imageRepo;
            _productItemRepo = productItemRepo;
            _sizeRepo = sizeRepo;
            _feedbackRepo = feedbackRepo;
        }

        public async Task<List<RentOrderDetailResModel>> GetRentOrderDetails(Guid RentOrderId)
        {
            List<TblRentOrderDetail> list = await _context.TblRentOrderDetails.Where(x => x.RentOrderId.Equals(RentOrderId)).ToListAsync();
            List<RentOrderDetailResModel> resultList = new();
            foreach (TblRentOrderDetail detail in list)
            {

                TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get((Guid)detail.ProductItemDetailId);
                TblSize? sizeGet = await _sizeRepo.Get(tblProductItemDetail.SizeId);
                List<string> imgGet = await _imageRepo.GetImgUrlProductItemDetail(tblProductItemDetail.Id);
                SizeResModel size = new()
                {
                    Id = sizeGet.Id,
                    SizeName = sizeGet.Name,
                    SizeType = sizeGet.Type
                };
                ProductItemDetailResModel upResult = new()
                {
                    Id = tblProductItemDetail.Id,
                    Size = size,
                    RentPrice = tblProductItemDetail.RentPrice,
                    SalePrice = tblProductItemDetail.SalePrice,
                    Quantity = tblProductItemDetail.Quantity,
                    Status = tblProductItemDetail.Status,
                    TransportFee = tblProductItemDetail.TransportFee,
                    ImagesURL = imgGet
                };
                TblImage image = await _imageRepo.GetImgUrlRentOrderDetail(detail.Id);
                string imageURl = "";
                if (image != null)
                {
                    imageURl = image.ImageUrl;
                }
                List<FeedbackOrderResModel> fbList = await _feedbackRepo.GetFeedBackOrderDetail(RentOrderId, (Guid)detail.ProductItemDetailId);
                 RentOrderDetailResModel model = new()
                {
                    ID = detail.Id,
                    ProductItemDetail = upResult ?? null,
                    Quantity = detail.Quantity ?? null,
                    TotalPrice = detail.TotalPrice ?? null,
                    RentPricePerUnit = detail.RentPricePerUnit ?? null,
                    SizeName = "" + detail.SizeName,
                    ProductItemName = "" + detail.ProductItemName,
                    ImgURL = imageURl,
                    FeedbackList = fbList
                 };
                resultList.Add(model);
            }
            return resultList;
        }

        public async Task<List<TblRentOrderDetail>> GetRentOrderDetailsByRentOrderID(Guid RentOrderId)
        {
            return await _context.TblRentOrderDetails.Where(x=>x.RentOrderId.Equals(RentOrderId)).ToListAsync();
        }

        public async Task<bool> UpdateRentOrderDetail(TblRentOrderDetail entity)
        {
            _ = _context.TblRentOrderDetails.Update(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }
    }
}

