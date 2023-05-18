using System;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboOrderRepo
{
	public interface ITakecareComboOrderRepo : IRepository<TblTakecareComboOrder>
    {
		Task<Page<TblTakecareComboOrder>> GetAllTakecreComboOrder(PaginationRequestModel paginationRequestModel);
        Task<bool> ChangeTakecareComboOrderStatus(Guid id, string status);

    }
}

