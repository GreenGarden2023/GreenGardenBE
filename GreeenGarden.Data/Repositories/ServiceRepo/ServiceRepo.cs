using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
    public class ServiceRepo : Repository<TblService>, IServiceRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IUserRepo _userRepo;
        private readonly IRewardRepo _rewardRepo;
        public ServiceRepo(GreenGardenDbContext context, IUserRepo userRepo, IRewardRepo rewardRepo) : base(context)
        {
            _context = context;
            _userRepo = userRepo;
            _rewardRepo = rewardRepo;
        }

        public async Task<bool> AssignTechnician(ServiceAssignModelManager serviceAssignModelManager)
        {
            try
            {
                TblService tblService = await _context.TblServices.Where(x => x.Id.Equals(serviceAssignModelManager.ServiceID)).FirstOrDefaultAsync();
                if (tblService != null)
                {
                    TblUser tblUser = await _userRepo.Get(serviceAssignModelManager.TechnicianID);
                    tblService.TechnicianId = serviceAssignModelManager.TechnicianID;
                    tblService.TechnicianName = tblUser.FullName;
                    _ = _context.Update(tblService);
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
                    }
                    else if (status.Trim().ToLower().Equals(ServiceStatus.REJECTED))
                    {
                        tblService.Status = ServiceStatus.REJECTED;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else if (status.Trim().ToLower().Equals(ServiceStatus.REPROCESS))
                    {
                        tblService.Status = ServiceStatus.REPROCESS;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else if(status.Trim().ToLower().Equals(ServiceStatus.CONFIRMED))
                    {
                        tblService.Status = ServiceStatus.CONFIRMED;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else if (status.ToLower().Equals(ServiceStatus.USER_APPROVED))
                    {
                        tblService.Status = ServiceStatus.USER_APPROVED;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else if (status.Trim().ToLower().Equals(ServiceStatus.REPROCESS))
                    {
                        tblService.Status = ServiceStatus.REPROCESS;
                        _ = _context.Update(tblService);
                        _ = await _context.SaveChangesAsync();
                        return true;
                    }
                    else {
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

        public async Task<bool> CheckServiceCode(string serviceCode)
        {
            TblService tblService = await _context.TblServices.Where(x => x.ServiceCode.Equals(serviceCode)).FirstOrDefaultAsync();
            if (tblService != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<TblService>> GetAllRequest()
        {
            List<TblService> tblServices = await _context.TblServices.ToListAsync();
            return tblServices;
        }

        public async Task<List<TblService>> GetRequestByUser(Guid userId)
        {
            List<TblService> tblServices = await _context.TblServices.Where(x => x.UserId.Equals(userId)).ToListAsync();
            return tblServices;
        }

        public async Task<TblService> GetServiceByServiceCode(string serviceCode)
        {
            TblService tblService = await _context.TblServices.Where(x => x.ServiceCode.Equals(serviceCode)).FirstOrDefaultAsync();
            if (tblService != null)
            {
                return tblService;
            }
            else {
                return null;
            }

        }

        public async Task<bool> UpdateServiceUserInfo(ServiceUpdateModelManager serviceUpdateModelManager)
        {
            try
            {
                TblService tblService = await _context.TblServices.Where(x => x.Id.Equals(serviceUpdateModelManager.ServiceID)).FirstOrDefaultAsync();
                if (tblService != null)
                {
                    int returnPoints = (int)(tblService.RewardPointUsed - serviceUpdateModelManager.RewardPointUsed);
                    _ = await _rewardRepo.UpdateUserRewardPointByUserID(tblService.UserId, returnPoints , 0);
;
                    if (!String.IsNullOrEmpty(serviceUpdateModelManager.Name) && !serviceUpdateModelManager.Name.Equals(tblService.Name))
                    {
                        tblService.Name = serviceUpdateModelManager.Name;
                    }
                    if (!String.IsNullOrEmpty(serviceUpdateModelManager.Phone) && !serviceUpdateModelManager.Phone.Equals(tblService.Phone))
                    {
                        tblService.Phone = serviceUpdateModelManager.Phone;
                    }
                    if (!String.IsNullOrEmpty(serviceUpdateModelManager.Email) && !serviceUpdateModelManager.Email.Equals(tblService.Email))
                    {
                        tblService.Email = serviceUpdateModelManager.Email;
                    }
                    if (!String.IsNullOrEmpty(serviceUpdateModelManager.Rules) && !serviceUpdateModelManager.Rules.Equals(tblService.Rules))
                    {
                        tblService.Rules = serviceUpdateModelManager.Rules;
                    }
                    if (!String.IsNullOrEmpty(serviceUpdateModelManager.Address) && !serviceUpdateModelManager.Address.Equals(tblService.Address))
                    {
                        tblService.Address = serviceUpdateModelManager.Address;
                    }
                    if (serviceUpdateModelManager.TransportFee != null && !serviceUpdateModelManager.TransportFee.Equals(tblService.TransportFee))
                    {
                        tblService.TransportFee = serviceUpdateModelManager.TransportFee;
                    }
                    if (serviceUpdateModelManager.RewardPointUsed != null && serviceUpdateModelManager.RewardPointUsed != 0 && !serviceUpdateModelManager.RewardPointUsed.Equals(tblService.RewardPointUsed))
                    {
                        tblService.RewardPointUsed = serviceUpdateModelManager.RewardPointUsed;
                    }
                    if (serviceUpdateModelManager.StartDate != null && !serviceUpdateModelManager.StartDate.Equals(tblService.StartDate))
                    {
                        tblService.StartDate = serviceUpdateModelManager.StartDate;
                    }
                    if (serviceUpdateModelManager.EndDate != null && !serviceUpdateModelManager.EndDate.Equals(tblService.EndDate))
                    {
                        tblService.EndDate = serviceUpdateModelManager.EndDate;
                    }
                    _ = _context.Update(tblService);
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

