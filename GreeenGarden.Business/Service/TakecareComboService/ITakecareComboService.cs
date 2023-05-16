using System;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboModel;

namespace GreeenGarden.Business.Service.TakecareComboService
{
	public interface ITakecareComboService
	{
		Task<ResultModel> InsertTakecareCombo(TakecareComboInsertModel takecareComboInsertModel, string token);
		Task<ResultModel> GetTakecareComboByID(Guid comboID);
        Task<ResultModel> GetTakecareCombos(string status);
		Task<ResultModel> UpdateTakecareCombo(TakecareComboUpdateModel takecareComboUpdateModel, string token);
    }
}

