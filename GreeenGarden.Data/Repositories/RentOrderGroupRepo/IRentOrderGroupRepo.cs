﻿using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RentOrderGroupRepo
{
    public interface IRentOrderGroupRepo : IRepository<TblRentOrderGroup>
    {
        Task<ResultModel> UpdateRentOrderGroup(Guid rentOrderGroupID, double newRentOrderAmount);
        Task<Page<TblRentOrderGroup>> GetRentOrderGroup(PaginationRequestModel paginationRequestModel, Guid userID);
        Task<Page<TblRentOrderGroup>> GetAllRentOrderGroup(PaginationRequestModel paginationRequestModel);
    }
}

