using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
	public class RentOrderRepo : Repository<TblRentOrder>, IRentOrderRepo
	{
		private readonly GreenGardenDbContext _context;
		public RentOrderRepo(GreenGardenDbContext context) :base(context)
		{
			_context = context;
		}

		public async Task<List<TblRentOrder>> GetRentOrders(Guid userID)
		{
			List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.UserId.Equals(userID)).ToListAsync();
			return listTblOrder;
		}
	}
}

