using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.DistrictRepo
{
    public interface IDistrictRepo : IRepository<TblDistrict>
    {
        Task<string> GetADistrict(int id);
        Task<string> GetNameDistrict(int id);
        Task<List<TblDistrict>> GetDistrictList();
    }
}

