using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboRepo
{
	public interface ITakecareComboRepo : IRepository<TblTakecareCombo>
    {
		Task<List<TblTakecareCombo>> GetAllTakecareCombo(string status);
		Task<ResultModel> UpdateTakecareCombo(TakecareComboUpdateModel takecareComboUpdateModel);
	}
}

