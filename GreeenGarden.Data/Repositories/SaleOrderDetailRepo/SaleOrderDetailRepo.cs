using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SaleOrderDetailRepo
{
	public class SaleOrderDetailRepo : Repository<TblSaleOrderDetail>, ISaleOrderDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        public SaleOrderDetailRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SaleOrderDetailResModel>> GetSaleOrderDetails(Guid saleOrderId)
        {
            List<TblSaleOrderDetail> list = await _context.TblSaleOrderDetails.Where(x => x.SaleOderId.Equals(saleOrderId)).ToListAsync();
            List<SaleOrderDetailResModel> resultList = new List<SaleOrderDetailResModel>();
            foreach (TblSaleOrderDetail detail in list)
            {
                SaleOrderDetailResModel model = new SaleOrderDetailResModel
                {
                    ID = detail.Id,
                    Quantity = detail.Quantity ?? null,
                    TotalPrice = detail.TotalPrice ?? null,
                    SalePricePerUnit = detail.SalePricePerUnit ?? null,
                    SizeName = "" + detail.SizeName,
                    ProductItemName = "" + detail.ProductItemName
                };
                resultList.Add(model);
            }
            return resultList;
        }
    }
}

