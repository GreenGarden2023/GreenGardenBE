using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SaleOrderDetailRepo
{
    public class SaleOrderDetailRepo : Repository<TblSaleOrderDetail>, ISaleOrderDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IImageRepo _imageRepo;
        public SaleOrderDetailRepo(GreenGardenDbContext context, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _imageRepo = imageRepo;
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
                SaleOrderDetailResModel model = new()
                {
                    ID = detail.Id,
                    Quantity = detail.Quantity ?? null,
                    TotalPrice = detail.TotalPrice ?? null,
                    SalePricePerUnit = detail.SalePricePerUnit ?? null,
                    SizeName = "" + detail.SizeName,
                    ProductItemName = "" + detail.ProductItemName,
                    ImgURL = imageURl
                };
                resultList.Add(model);
            }
            return resultList;
        }
    }
}

