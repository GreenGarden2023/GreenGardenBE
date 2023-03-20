using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.DistrictRepo
{
    public class DistrictRepo : Repository<TblDistrict>, IDistrictRepo
    {
        private readonly GreenGardenDbContext _context;
        public DistrictRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> GetADistrict(int id)
        {
            TblDistrict district = await _context.TblDistricts.FirstOrDefaultAsync();
            return district != null ? district.DistrictName : "";

        }

        public async Task<List<TblDistrict>> GetDistrictList()
        {
            List<TblDistrict> districts = await _context.TblDistricts.ToListAsync();
            return districts;
        }
    }
}

