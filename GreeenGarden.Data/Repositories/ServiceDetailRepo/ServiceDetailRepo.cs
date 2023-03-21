using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ServiceRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceDetailRepo
{
	public class ServiceDetailRepo : Repository<TblServiceDetail>, IServiceDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        public ServiceDetailRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ServiceDetailResModel>> GetServiceDetailByServiceID(Guid serviceID)
        {
            try
            {
                List<TblServiceDetail> tblServiceDetails = await _context.TblServiceDetails.Where(x => x.ServiceId.Equals(serviceID)).ToListAsync();
                if (tblServiceDetails.Any())
                {
                    List<ServiceDetailResModel> resList = new List<ServiceDetailResModel>();
                    foreach (TblServiceDetail detail in tblServiceDetails)
                    {
                        ServiceDetailResModel serviceDetail = new ServiceDetailResModel
                        {
                            ID = detail.Id,
                            UserTreeID = detail.UserTreeId ?? Guid.Empty,
                            ServiceID = detail.ServiceId ?? Guid.Empty,
                            TreeName = detail.TreeName ?? "",
                            Description = detail.Desciption ?? "",
                            Quantity = detail.Quantity ?? 0
                        };
                        resList.Add(serviceDetail);
                    }
                    return resList;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }
    }
}

