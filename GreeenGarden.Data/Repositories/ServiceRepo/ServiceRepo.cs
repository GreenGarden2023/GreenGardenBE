﻿using System;
using System.Net.NetworkInformation;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
	public class ServiceRepo : Repository<TblService>, IServiceRepo
	{
		private readonly GreenGardenDbContext _context;
		private readonly IUserRepo _userRepo;
		public ServiceRepo(GreenGardenDbContext context, IUserRepo userRepo) : base(context)
		{
			_context = context;
			_userRepo = userRepo;
		}

        public async Task<bool> AssignTechnician(ServiceAssignModelManager serviceAssignModelManager)
        {
			try { 
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
    }
}

