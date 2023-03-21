using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
	public class ServiceRepo : Repository<TblService>, IServiceRepo
	{
		private readonly GreenGardenDbContext _context;
		public ServiceRepo(GreenGardenDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<bool> ChangeServiceStatus(Guid serviceId, string status)
        {
			try
			{
				TblService tblService = await _context.TblServices.Where(x => x.Id.Equals(serviceId)).FirstOrDefaultAsync();
				if (tblService != null)
				{
					if (status.Trim().ToLower().Equals(ServiceStatus.ACCEPTED))
					{
						tblService.Status = ServiceStatus.ACCEPTED;
						_ = _context.Update(tblService);
						_ = await _context.SaveChangesAsync();
						return true;
					}else if (status.Trim().ToLower().Equals(ServiceStatus.REJECTED))
					{
                        tblService.Status = ServiceStatus.REJECTED;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
        }
    }
}

