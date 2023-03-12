using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.SaleOrderDetailRepo
{
	public interface ISaleOrderDetailRepo : IRepository<TblSaleOrderDetail>
    {
        Task<List<SaleOrderDetailResModel>> GetSaleOrderDetails(Guid saleOrderId);
    }
}

