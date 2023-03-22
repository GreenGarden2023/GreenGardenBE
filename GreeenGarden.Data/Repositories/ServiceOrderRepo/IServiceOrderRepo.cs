using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
	public interface IServiceOrderRepo : IRepository<TblServiceOrder>
    {
        Task<bool> CheckOrderCode(string Code);
        Task<Page<TblServiceOrder>> GetAllServiceOrders(PaginationRequestModel paginationRequestModel);
        Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID);
    }
}

