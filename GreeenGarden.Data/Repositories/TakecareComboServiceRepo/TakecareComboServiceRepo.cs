using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GreeenGarden.Data.Repositories.TakecareComboServiceRepo
{


    public class TakecareComboServiceRepo : Repository<TblTakecareComboService>, ITakecareComboServiceRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IUserRepo _userRepo;
        public TakecareComboServiceRepo(GreenGardenDbContext context, IUserRepo userRepo) : base(context)
        {
            _context = context;
            _userRepo = userRepo;
        }

        public async Task<bool> AssignTechnicianTakecareComboService(Guid takecareComboServiceID, Guid technicianID)
        {
            try
            {
                TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Id.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                TblUser user = await _userRepo.Get(technicianID);
                tblTakecareComboService.TechnicianId = user.Id;
                tblTakecareComboService.TechnicianName = user.FullName;
                _ = _context.Update(tblTakecareComboService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelService(Guid takecareComboServiceID, string cancelReason, Guid cancelBy)
        {
            try
            {
                TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Id.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                tblTakecareComboService.Status = TakecareComboServiceStatus.CANCEL;
                tblTakecareComboService.CancelReason = cancelReason;
                tblTakecareComboService.CancelBy = cancelBy;
                _ = _context.Update(tblTakecareComboService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangeTakecareComboServiceStatus(Guid takecareComboServiceID, string status)
        {
            try
            {
                TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Id.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                tblTakecareComboService.Status = status;
                _ = _context.Update(tblTakecareComboService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckCodeDup(string code)
        {
            TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Code.Equals(code)).FirstOrDefaultAsync();
            if (tblTakecareComboService != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Page<TblTakecareComboOrder>> GetAllServiceOrderByRangDate(PaginationRequestModel pagingModel, DateTime fromDate, DateTime toDate)
        {
            return await _context.TblTakecareComboOrders.Where(x => x.ServiceEndDate >= fromDate && x.ServiceEndDate <= toDate)
                .OrderByDescending(x => x.ServiceEndDate)
                .PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
        }

        public async Task<List<TblTakecareComboService>> GetAllTakecareComboService(string status)
        {
            if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.PENDING))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.PENDING)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.ACCEPTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.ACCEPTED)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.REJECTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.REJECTED)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
        }

        public async Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByCustomer(string status, Guid userId)
        {
            if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.PENDING))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.PENDING) && x.UserId.Equals(userId)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.ACCEPTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.ACCEPTED) && x.UserId.Equals(userId)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.REJECTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.REJECTED) && x.UserId.Equals(userId)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.UserId.Equals(userId)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
        }

        public async Task<List<TblTakecareComboService>> GetAllTakecareComboServiceByTech(string serviceCode, string status, Guid technician)
        {
            if (serviceCode != null)
            {
                if (status == "all" || status== null)
                {
                    return await _context.TblTakecareComboServices.Where(x => x.TechnicianId.Equals(technician) && x.Code.Equals(serviceCode)).OrderByDescending(y => y.CreateDate).ToListAsync();
                }
                else
                {
                    return await _context.TblTakecareComboServices.Where(x => x.TechnicianId.Equals(technician) && x.Status.Equals(status) && x.Code.Equals(serviceCode)).OrderByDescending(y => y.CreateDate).ToListAsync();
                }
            }
            if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.PENDING))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.PENDING) && x.TechnicianId.Equals(technician)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.ACCEPTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.ACCEPTED) && x.TechnicianId.Equals(technician)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else if (status.Trim().ToLower().Equals(TakecareComboServiceStatus.REJECTED))
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Status.Equals(TakecareComboServiceStatus.REJECTED) && x.TechnicianId.Equals(technician)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
            else
            {
                List<TblTakecareComboService> tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.TechnicianId.Equals(technician)).OrderByDescending(y => y.CreateDate).ToListAsync();
                return tblTakecareComboService;
            }
        }

        public async Task<TblTakecareComboService> GetTakecareComboServiceByCode(string code)
        {
            return await _context.TblTakecareComboServices.Where(x => x.Code.Equals(code)).FirstOrDefaultAsync();
        }

        public async Task<Page<TblTakecareComboService>> GetTakecareComboServiceByPhone(GetServiceByPhoneModel model, PaginationRequestModel pagingModel)
        {
            if (model.Status == "all" || model.Status == null)
            {
                return await _context.TblTakecareComboServices.Where(x => x.Phone.Contains(model.Phone)).OrderBy(x => x.CreateDate).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
            else
            {
                return await _context.TblTakecareComboServices.Where(x => x.Phone.Contains(model.Phone) && x.Status.Equals(model.Status)).OrderBy(x => x.CreateDate).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
            }
        }

        public async Task<bool> RejectService(Guid takecareComboServiceID, string reason, Guid userId)
        {
            try
            {
                TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Id.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                tblTakecareComboService.Status = TakecareComboServiceStatus.REJECTED;
                tblTakecareComboService.CancelBy = userId;
                tblTakecareComboService.CancelReason = reason;
                _ = _context.Update(tblTakecareComboService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateServiceOrderCareGuide(Guid takecareComboServiceOrderID, string careGuideUrl)
        {
            var order = await _context.TblTakecareComboOrders.Where(x => x.Id.Equals(takecareComboServiceOrderID)).FirstOrDefaultAsync();
            if (careGuideUrl != null)
            {
                order.CareGuideUrl = careGuideUrl;
                _context.TblTakecareComboOrders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateTakecareComboService(TakecareComboServiceUpdateModel takecareComboServiceUpdateModel)
        {
            try
            {
                TblTakecareComboService tblTakecareComboService = await _context.TblTakecareComboServices.Where(x => x.Id.Equals(takecareComboServiceUpdateModel.Id)).FirstOrDefaultAsync();
                if (takecareComboServiceUpdateModel.TakecareComboId != null && !takecareComboServiceUpdateModel.TakecareComboId.Equals(tblTakecareComboService.TakecareComboId))
                {
                    tblTakecareComboService.TakecareComboId = (Guid)takecareComboServiceUpdateModel.TakecareComboId;
                }
                if (!String.IsNullOrEmpty(takecareComboServiceUpdateModel.Name) && !takecareComboServiceUpdateModel.Name.Equals(tblTakecareComboService.Name))
                {
                    tblTakecareComboService.Name = takecareComboServiceUpdateModel.Name;
                }
                if (!String.IsNullOrEmpty(takecareComboServiceUpdateModel.Phone) && !takecareComboServiceUpdateModel.Phone.Equals(tblTakecareComboService.Phone))
                {
                    tblTakecareComboService.Phone = takecareComboServiceUpdateModel.Phone;
                }
                if (!String.IsNullOrEmpty(takecareComboServiceUpdateModel.Email) && !takecareComboServiceUpdateModel.Email.Equals(tblTakecareComboService.Email))
                {
                    tblTakecareComboService.Email = takecareComboServiceUpdateModel.Email;
                }
                if (!String.IsNullOrEmpty(takecareComboServiceUpdateModel.Address) && !takecareComboServiceUpdateModel.Address.Equals(tblTakecareComboService.Address))
                {
                    tblTakecareComboService.Address = takecareComboServiceUpdateModel.Address;
                }
                if (takecareComboServiceUpdateModel.TreeQuantity != null && takecareComboServiceUpdateModel.TreeQuantity != tblTakecareComboService.TreeQuantity)
                {
                    tblTakecareComboService.TreeQuantity = (int)takecareComboServiceUpdateModel.TreeQuantity;
                }
                if (takecareComboServiceUpdateModel.CareGuide != null && takecareComboServiceUpdateModel.CareGuide != tblTakecareComboService.CareGuide)
                {
                    tblTakecareComboService.CareGuide = takecareComboServiceUpdateModel.CareGuide;
                }
                if (takecareComboServiceUpdateModel.IsAtShop != null && takecareComboServiceUpdateModel.IsAtShop != tblTakecareComboService.IsAtShop)
                {
                    tblTakecareComboService.IsAtShop = takecareComboServiceUpdateModel.IsAtShop;
                }

                if (takecareComboServiceUpdateModel.StartDate != null && takecareComboServiceUpdateModel.NumOfMonth == null)
                {
                    tblTakecareComboService.StartDate = DateTime.ParseExact(takecareComboServiceUpdateModel.StartDate, "dd/MM/yyyy", null);
                    tblTakecareComboService.EndDate = tblTakecareComboService.StartDate.AddMonths(tblTakecareComboService.NumberOfMonths);

                }
                else if (takecareComboServiceUpdateModel.StartDate == null && takecareComboServiceUpdateModel.NumOfMonth != null)
                {
                    tblTakecareComboService.NumberOfMonths = (int)takecareComboServiceUpdateModel.NumOfMonth;
                    tblTakecareComboService.EndDate = tblTakecareComboService.StartDate.AddMonths(tblTakecareComboService.NumberOfMonths);

                }
                else if (takecareComboServiceUpdateModel.StartDate != null && takecareComboServiceUpdateModel.NumOfMonth != null)
                {
                    tblTakecareComboService.StartDate = DateTime.ParseExact(takecareComboServiceUpdateModel.StartDate, "dd/MM/yyyy", null);
                    tblTakecareComboService.NumberOfMonths = (int)takecareComboServiceUpdateModel.NumOfMonth;
                    tblTakecareComboService.EndDate = tblTakecareComboService.StartDate.AddMonths(tblTakecareComboService.NumberOfMonths);
                }
                else
                {
                }

                _ = _context.Update(tblTakecareComboService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                e.ToString();
                return false;
            }
        }
    }
}

