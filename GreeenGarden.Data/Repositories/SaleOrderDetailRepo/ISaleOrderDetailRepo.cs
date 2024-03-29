﻿using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SaleOrderDetailRepo
{
    public interface ISaleOrderDetailRepo : IRepository<TblSaleOrderDetail>
    {
        Task<List<SaleOrderDetailResModel>> GetSaleOrderDetails(Guid saleOrderId);
        Task<bool> UpdateSaleOrderDetails(TblSaleOrderDetail entity);
        Task<List<TblSaleOrderDetail>> GetSaleOrderDetailByOrderId(Guid saleOrderId);
    }
}

