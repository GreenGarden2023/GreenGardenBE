﻿using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public interface IServiceOrderRepo : IRepository<TblServiceOrder>
    {
        Task<bool> CheckOrderCode(string Code);
        Task<ResultModel> UpdateServiceOrderDeposit(Guid serviceOrderID);
        Task<ResultModel> UpdateServiceOrderRemain(Guid serviceOrderID, double amount);
        Task<Page<TblServiceOrder>> GetAllServiceOrders(PaginationRequestModel paginationRequestModel);
        Task<Page<TblServiceOrder>> GetAllServiceOrderByRangDate(PaginationRequestModel paginationRequestModel, DateTime fromDate, DateTime toDate);
        Task<Page<TblServiceOrder>> SearchServiceOrder(OrderFilterModel model, PaginationRequestModel pagingModel);
        Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID);
        Task<TblServiceOrder> GetServiceOrderByOrderCode(string orderCode);
        Task<Page<TblServiceOrder>> GetServiceOrderByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID);
        Task<Page<TblServiceOrder>> GetServiceOrderByTechnicianToday(PaginationRequestModel paginationRequestModel, Guid technicianID, string? status, bool nextDate);
        Task<TblServiceOrder> GetServiceOrderByServiceID(Guid serviceId);
        Task<bool> UpdateServiceOrder(TblServiceOrder entity);
        Task<bool> UpdateServiceOrderCareGuide(Guid orderID, string careGuideURL);
        Task<bool> CompleteServiceOrder(Guid serviceOrderID);
    }
}

