using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceDetailRepo
{
	public class ServiceDetailRepo : Repository<TblServiceDetail>, IServiceDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IImageRepo _imageRepo; 
        public ServiceDetailRepo(GreenGardenDbContext context, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _imageRepo = imageRepo;
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
                        List<string> imgs = await _imageRepo.GetImgUrlServiceDetail(detail.Id);
                        ServiceDetailResModel serviceDetail = new ServiceDetailResModel
                        {
                            ID = detail.Id,
                            UserTreeID = detail.UserTreeId ?? Guid.Empty,
                            ServiceID = detail.ServiceId ?? Guid.Empty,
                            TreeName = detail.TreeName ?? "",
                            Description = detail.Desciption ?? "",
                            Quantity = detail.Quantity ?? 0,
                            ServicePrice = detail.ServicePrice ?? 0,
                            ManagerDescription = detail.ManagerDescription,
                            ImgUrls = imgs
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

        public async Task<bool> UpdateServiceDetailManager(ServiceDetailManagerUpdateModel serviceDetail)
        {
            try
            {
                TblServiceDetail tblServiceDetail = await _context.TblServiceDetails.Where(x => x.Id.Equals(serviceDetail.ServiceDetailID)).FirstOrDefaultAsync();
                if(tblServiceDetail != null)
                {
                    if(serviceDetail.Quantity != null && serviceDetail.Quantity != tblServiceDetail.Quantity)
                    {
                        tblServiceDetail.Quantity = serviceDetail.Quantity;
                    }
                    if (serviceDetail.ServicePrice != null && serviceDetail.ServicePrice != tblServiceDetail.ServicePrice)
                    {
                        tblServiceDetail.ServicePrice = serviceDetail.ServicePrice;
                    }
                    if (serviceDetail.ManagerDescription != null && serviceDetail.ManagerDescription != tblServiceDetail.ManagerDescription)
                    {
                        tblServiceDetail.ManagerDescription = serviceDetail.ManagerDescription;
                    }
                    _ = _context.Update(tblServiceDetail);
                    _ = await _context.SaveChangesAsync();
                    return true;

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

