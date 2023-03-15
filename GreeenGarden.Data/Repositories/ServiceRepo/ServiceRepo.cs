using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
    public class ServiceRepo : Repository<TblService>, IServiceRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ServiceRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<bool> ChangeStatusService(Guid serviceID, string status)
        {
            var service = await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
            service.Status= status;
            _context.TblServices.Update(service);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteServiceUserTree(TblServiceUserTree entities)
        {
            _context.TblServiceUserTrees.Remove(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetailServiceByCustomerResModel> GetDetailServiceByCustomer(Guid serviceID)
        {
            var result = new DetailServiceByCustomerResModel();
            var service = await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
            var user = await _context.TblUsers.Where(x => x.Id.Equals(service.UserId)).FirstOrDefaultAsync();

            var userRecord = new UserResModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                Mail = user.Mail,
            };
            result.User = userRecord;
            result.Services = new ServiceForListResModel();
            result.Services.Id = service.Id;
            result.Services.StartDate = service.StartDate;
            result.Services.EndDate = service.EndDate;
            result.Services.Mail = service.Mail;
            result.Services.Phone = service.Phone;
            result.Services.Address = service.Address;
            result.Services.Status = service.Status;
            result.Services.UserTrees = new List<ServiceUserTreeRespModel>();

            var listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(service.Id)).ToListAsync();
            foreach (var sut in listSut)
            {
                var ut = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                var utRecord = new UserTreeResModel();
                utRecord.Id = ut.Id;
                utRecord.TreeName = ut.TreeName;
                utRecord.Description = ut.Description;
                utRecord.Quantity = ut.Quantity;
                utRecord.Status = ut.Status;
                utRecord.ImageUrl = await getImgUrlByUserTreeID(ut.Id);

                var sutRecord = new ServiceUserTreeRespModel()
                {
                    ID = sut.Id,
                    UserTree = utRecord,
                    Quantity = sut.Quantity,
                    Price = sut.Price
                };
                result.Services.UserTrees.Add(sutRecord);
            }
            return result;
        }

        public async Task<List<string>> getImgUrlByUserTreeID(Guid userTreeID)
        {
            var result = new List<string>();
            var listImg = await _context.TblImages.Where(x => x.UserTreeId.Equals(userTreeID)).ToListAsync();
            foreach (var i in listImg)
            {
                result.Add(i.ImageUrl);
            }
            return result;
        }

        public async Task<ListServiceByCustomerResModel> GetListServiceByCustomer(TblUser user)
        {
            var result = new ListServiceByCustomerResModel();
            var userRecord = new UserResModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                Mail = user.Mail,                
            };
            result.User = userRecord;
            result.Services = new List<ServiceForListResModel>();
            var listService = await _context.TblServices.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            foreach (var s in listService)
            {
                var serviceRecord = new ServiceForListResModel();
                serviceRecord.Id = s.Id;
                serviceRecord.StartDate = s.StartDate;
                serviceRecord.EndDate = s.EndDate;
                serviceRecord.Mail = s.Mail;
                serviceRecord.Phone = s.Phone;
                serviceRecord.Address = s.Address;
                serviceRecord.Status = s.Status;
                serviceRecord.UserTrees = new List<ServiceUserTreeRespModel>();                

                var listServiceUserTree = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                foreach (var sut in listServiceUserTree)
                {
                    var ut = await _context.TblUserTrees.Where(x=>x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                    var utRecord = new UserTreeResModel();
                    utRecord.Id = ut.Id;
                    utRecord.TreeName = ut.TreeName;
                    utRecord.Description = ut.Description;
                    utRecord.Quantity = ut.Quantity;
                    utRecord.Status = ut.Status;
                    utRecord.ImageUrl = await getImgUrlByUserTreeID(ut.Id);

                    var sutRecord = new ServiceUserTreeRespModel();
                    sutRecord.ID= sut.Id;
                    sutRecord.Quantity = sut.Quantity;
                    sutRecord.Price = sut.Price;
                    sutRecord.UserTree = utRecord;

                    serviceRecord.UserTrees.Add(sutRecord);
                }
                result.Services.Add(serviceRecord);
            }
            return result;
        }

        public async Task<TblService> GetTblService(Guid serviceID)
        {
            return  await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
        }

        public async Task<List<TblServiceUserTree>> GetListTblServiceUserTree(Guid serviceID)
        {
            return await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(serviceID)).ToListAsync();
        }

        public async Task<TblUser> getTblUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<bool> insertServiceUserTree(TblServiceUserTree entities)
        {
            await _context.TblServiceUserTrees.AddAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateService(TblService entities)
        {
            _context.TblServices.Update(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateServiceUserTree(TblServiceUserTree entities)
        {
            _context.TblServiceUserTrees.Update(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TblServiceUserTree> GetTblServiceUserTree(Guid SerUtID)
        {
            return await _context.TblServiceUserTrees.Where(x => x.Id.Equals(SerUtID)).FirstOrDefaultAsync();
        }

        public async Task<TblUserTree> getUserTreeByID(Guid userTreeID)
        {
            return await _context.TblUserTrees.Where(x => x.Id.Equals(userTreeID)).FirstOrDefaultAsync();
        }
    }
}
