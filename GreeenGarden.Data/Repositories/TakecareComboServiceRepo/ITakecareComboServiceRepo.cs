﻿using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.TakecareComboServiceRepo
{
	public interface ITakecareComboServiceRepo : IRepository<TblTakecareComboService>
    {
		Task<bool> CheckCodeDup(string code);
		Task<List<TblTakecareComboService>> GetAllTakecareComboService(string status);
		Task<bool> ChangeTakecareComboServiceStatus(Guid takecareComboServiceID, string status);
		Task<bool> AssignTechnicianTakecareComboService(Guid takecareComboServiceID, Guid technicianID);
		Task<bool> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel);
    }
}
