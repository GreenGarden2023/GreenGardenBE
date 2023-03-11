using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
	public class RentOrderRepo : Repository<TblRentOrder>, IRentOrderRepo
	{
		private readonly GreenGardenDbContext _context;
		public RentOrderRepo(GreenGardenDbContext context) :base(context)
		{
			_context = context;
		}
	}
}

