using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.SaleOrderRepo
{
	public interface ISaleOrderRepo : IRepository<TblSaleOrder>
    {
        Task<List<TblSaleOrder>> GetSaleOrders(Guid userID);
        Task<ResultModel> CancelSaleOrder(Guid SaleOrderID);
    }
}

