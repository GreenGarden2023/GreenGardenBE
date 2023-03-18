using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.DistrictRepo
{
	public interface IDistrictRepo : IRepository<TblDistrict>
	{
		Task<string> GetADistrict(int id);
		Task<List<TblDistrict>> GetDistrictList();
	}
}

