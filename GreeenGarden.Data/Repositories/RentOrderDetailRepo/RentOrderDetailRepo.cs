using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.Repository;
using GreeenGarden.Data.Repositories.ProductItemDetailRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RentOrderDetailRepo
{
	public class RentOrderDetailRepo : Repository<TblRentOrderDetail> , IRentOrderDetailRepo
	{
		private readonly GreenGardenDbContext _context;
		private readonly IProductItemDetailRepo _productItemDetailRepo;
        private readonly IProductItemRepo _productItemRepo;
        private readonly IImageRepo _imageRepo;
		public RentOrderDetailRepo(GreenGardenDbContext context, IProductItemDetailRepo sizeProductItemRepo, IImageRepo imageRepo, IProductItemRepo productItemRepo) : base(context) 
		{
			_context = context;
			_productItemDetailRepo = sizeProductItemRepo;
			_imageRepo = imageRepo;
			_productItemRepo = productItemRepo;
		}

        public async Task<List<RentOrderDetailResModel>> GetRentOrderDetails(Guid RentOrderId)
        {
			List<TblRentOrderDetail> list = await _context.TblRentOrderDetails.Where(x => x.RentOrderId.Equals(RentOrderId)).ToListAsync();
			List<RentOrderDetailResModel> resultList = new List<RentOrderDetailResModel>();
			foreach(TblRentOrderDetail detail in list)
			{
				TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get(detail.ProductItemDetailId);
				TblProductItem tblProductItem = await _productItemRepo.Get(tblProductItemDetail.ProductItemId);
				TblImage tblImage = await _imageRepo.GetImgUrlProductItem(tblProductItem.Id);
				RentOrderDetailResModel model = new RentOrderDetailResModel
				{
					ID = detail.Id,
					productItemDetailID = detail.ProductItemDetailId,
					productItemName = tblProductItem.Name,
					productItemUrl = "" + tblImage.ImageUrl,
                    productItemDetailTotalPrice = (double)detail.ProductItemDetailTotalPrice,
					Quantity = (int)detail.Quantity
				};
				resultList.Add(model);
            }
			return resultList;
        }
    }
}

