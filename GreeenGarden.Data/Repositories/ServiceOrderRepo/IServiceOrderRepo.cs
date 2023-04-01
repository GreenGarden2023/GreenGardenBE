using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
	public interface IServiceOrderRepo : IRepository<TblServiceOrder>
    {
        Task<bool> CheckOrderCode(string Code);
        Task<ResultModel> UpdateServiceOrderDeposit(Guid serviceOrderID);
        Task<ResultModel> UpdateServiceOrderRemain(Guid serviceOrderID, double amount);
        Task<Page<TblServiceOrder>> GetAllServiceOrders(PaginationRequestModel paginationRequestModel);
        Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID);
        Task<Page<TblServiceOrder>> GetServiceOrderByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID);
        Task<TblServiceOrder> GetServiceOrderByServiceID( Guid serviceId);
        Task<bool> UpdateServiceOrder(TblServiceOrder entity);
        Task<bool> CompleteServiceOrder(Guid serviceOrderID);
    }
}

