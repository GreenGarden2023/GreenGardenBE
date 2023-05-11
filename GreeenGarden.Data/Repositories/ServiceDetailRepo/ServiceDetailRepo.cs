using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
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

        public async Task<ServiceDetailResModel?> GetServiceDetailByID(Guid serviceDetailID)
        {
            try
            {
                TblServiceDetail tblServiceDetail = await _context.TblServiceDetails.Where(x => x.Id.Equals(serviceDetailID)).FirstOrDefaultAsync();
                if (tblServiceDetail != null)
                {
                    List<string> imgs = await _imageRepo.GetImgUrlServiceDetail(tblServiceDetail.Id);
                    ServiceDetailResModel serviceDetail = new()
                    {
                        ID = tblServiceDetail.Id,
                        UserTreeID = tblServiceDetail.UserTreeId ?? Guid.Empty,
                        ServiceID = tblServiceDetail.ServiceId ?? Guid.Empty,
                        TreeName = tblServiceDetail.TreeName ?? "",
                        Description = tblServiceDetail.Desciption ?? "",
                        Quantity = tblServiceDetail.Quantity ?? 0,
                        ServicePrice = tblServiceDetail.ServicePrice ?? 0,
                        ManagerDescription = tblServiceDetail.ManagerDescription,
                        CareGuide = tblServiceDetail.CareGuide ?? null,
                        ImgUrls = imgs
                    };
                    return serviceDetail;
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

        public async Task<List<ServiceDetailResModel>?> GetServiceDetailByServiceID(Guid serviceID)
        {
            try
            {
                List<TblServiceDetail> tblServiceDetails = await _context.TblServiceDetails.Where(x => x.ServiceId.Equals(serviceID)).ToListAsync();
                if (tblServiceDetails.Any())
                {
                    List<ServiceDetailResModel> resList = new();

                    foreach (TblServiceDetail detail in tblServiceDetails)
                    {
                        List<string> imgs = await _imageRepo.GetImgUrlServiceDetail(detail.Id);
                        ServiceDetailResModel serviceDetail = new()
                        {
                            ID = detail.Id,
                            UserTreeID = detail.UserTreeId ?? Guid.Empty,
                            ServiceID = detail.ServiceId ?? Guid.Empty,
                            TreeName = detail.TreeName ?? "",
                            Description = detail.Desciption ?? "",
                            Quantity = detail.Quantity ?? 0,
                            ServicePrice = detail.ServicePrice ?? 0,
                            ManagerDescription = detail.ManagerDescription,
                            CareGuide = detail.CareGuide ?? null,
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

        public async Task<bool> UpdateCareGuideByUserTree(UpdateCareGuideByTechnModel model)
        {
            try
            {
                var serviceOrder = await _context.TblServiceOrders.Where(x => x.Id.Equals(model.OrderID)).FirstOrDefaultAsync();
                var service = await _context.TblServices.Where(x => x.Id.Equals(serviceOrder.ServiceId)).FirstOrDefaultAsync();
                foreach (var m in model.listCareGuide)
                {
                    var serviceDetail = await _context.TblServiceDetails.Where(x=>x.ServiceId.Equals(service.Id) 
                    && x.UserTreeId.Equals(m.UserTreeID)).FirstOrDefaultAsync();
                    if (serviceDetail != null)
                    {
                        serviceDetail.CareGuide = m.CareGuide;
                        _context.TblServiceDetails.Update(serviceDetail);
                        await _context.SaveChangesAsync();  
                    }
                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateServiceDetailManager(ServiceDetailUpdateModelManager serviceDetail)
        {
            try
            {
                TblServiceDetail tblServiceDetail = await _context.TblServiceDetails.Where(x => x.Id.Equals(serviceDetail.ServiceDetailID)).FirstOrDefaultAsync();
                if (tblServiceDetail != null)
                {
                    if (serviceDetail.Quantity != null && serviceDetail.Quantity != tblServiceDetail.Quantity)
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
                    if (serviceDetail.CareGuide != null && serviceDetail.CareGuide != tblServiceDetail.CareGuide)
                    {
                        tblServiceDetail.CareGuide = serviceDetail.CareGuide;
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

