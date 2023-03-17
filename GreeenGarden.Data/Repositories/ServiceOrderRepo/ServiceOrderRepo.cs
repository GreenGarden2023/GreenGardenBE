using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public class ServiceOrderRepo : Repository<TblServiceOrder>, IServiceOrderRepo
    {
        private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ServiceOrderRepo(IMapper mapper, GreenGardenDbContext context) : base(context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> createServiceOrder(TblServiceOrder entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;    
        }

        public async Task<bool> DeleteServiceUserTree(TblServiceUserTree entity)
        {
            _context.TblServiceUserTrees.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetailServiceOrderResModel> GetDetailServiceOrder(Guid SerOrderID)
        {
            var result = new DetailServiceOrderResModel();

            var soData = await _context.TblServiceOrders.Where(x => x.Id.Equals(SerOrderID)).FirstOrDefaultAsync();
            var userData = await _context.TblUsers.Where(x => x.Id.Equals(soData.UserId)).FirstOrDefaultAsync();
            result.User = _mapper.Map < UserResModel>(userData);
            result.ServiceOrder = _mapper.Map <ServiceOrderResModel>(soData);

            var techData = await _context.TblUsers.Where(x => x.Id.Equals(soData.TechnicianId)).FirstOrDefaultAsync();
            result.ServiceOrder.Technician = _mapper.Map<UserResModel>(techData);

            var sData = await _context.TblServices.Where(x => x.Id.Equals(soData.ServiceId)).FirstOrDefaultAsync();
            result.ServiceOrder.Service = _mapper.Map<ServiceResModel>(sData);

            result.ServiceOrder.Service.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
            var listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(sData.Id)).ToListAsync();
            foreach (var sut in listSut)
            {
                var sutRecord = new ServiceUserTreeRespModel();
                sutRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);

                var utData = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                var utRecord = new UserTreeResModel();
                utRecord = _mapper.Map<UserTreeResModel>(utData);
                utRecord.ImageUrl = await getImgUrlByUserTreeID(utData.Id);

                sutRecord.UserTree = utRecord;
                result.ServiceOrder.Service.ServiceUserTrees.Add(sutRecord);
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

        public async Task<ListServiceOrderResModel> GetListServiceOrder(TblUser user)
        {
            var result = new ListServiceOrderResModel();
            var listServiceOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            result.User = _mapper.Map<UserResModel>(user);
            result.ServiceOrder = new List<ServiceOrderResModel>();
            foreach (var so in listServiceOrder)
            {
                var ServiceOrderRes = new ServiceOrderResModel();
                var s = await _context.TblServices.Where(x => x.Id.Equals(so.ServiceId)).FirstOrDefaultAsync();
                var listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();

                var ServiceRes = _mapper.Map<ServiceOrderResModel>(s);

                ServiceOrderRes.Service.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
                foreach (var sut in listSut)
                {
                    var ServiceUserTreesRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);
                    ServiceOrderRes.Service.ServiceUserTrees.Add(ServiceUserTreesRecord);
                }
                result.ServiceOrder.Add(ServiceOrderRes);
            }
            return result;
        } // chưa test

        public async Task<List<ServiceOrderResManagerModel>> GetListServiceOrderByManager()
        {
            var result = new List<ServiceOrderResManagerModel>();
            var listSo = await _context.TblServiceOrders.ToListAsync();
            foreach (var so in listSo) 
            {
                var tech = await _context.TblUsers.Where(x => x.Id.Equals(so.TechnicianId)).FirstOrDefaultAsync();
                var user = await _context.TblUsers.Where(x => x.Id.Equals(so.UserId)).FirstOrDefaultAsync();
                var s = await _context.TblServices.Where(x => x.Id.Equals(so.ServiceId)).FirstOrDefaultAsync();
                var listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                UserResModel techRecord = _mapper.Map<UserResModel>(tech);
                UserResModel userRecord = _mapper.Map<UserResModel>(user);
                ServiceResModel serRecord = _mapper.Map<ServiceResModel>(s);
                serRecord.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
                foreach (var sut in listSut)
                {
                    ServiceUserTreeRespModel sutRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);
                    serRecord.ServiceUserTrees.Add(sutRecord);
                }
                ServiceOrderResManagerModel soRecord = _mapper.Map<ServiceOrderResManagerModel>(so);
                soRecord.Technician = techRecord;
                soRecord.User = userRecord;
                soRecord.Service = serRecord;
                result.Add(soRecord);
            }
            return result;

        }

        public async Task<List<TblServiceUserTree>> GetListTblServiceUserTreeByServiceID(Guid serviceID)
        {
            return await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(serviceID)).ToListAsync();
        }

        public async Task<List<UserResModel>> getListTecnician()
        {
            var result = new List<UserResModel>();
            var role = await _context.TblRoles.Where(x => x.RoleName.Equals("Technician")).FirstOrDefaultAsync();
            var listTech = await _context.TblUsers.Where(x=>x.RoleId.Equals(role.Id)).ToListAsync();
            foreach (var tech in listTech)
            {
                result.Add(_mapper.Map<UserResModel>(tech));
            }
            return result;
        }

        public async Task<TblService> GetTblServiceByID(Guid serviceID)
        {
            return await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
        }

        public async Task<TblServiceOrder> GetTblServiceOrderByID(Guid serOderID)
        {
            return await _context.TblServiceOrders.Where(x => x.Id.Equals(serOderID)).FirstOrDefaultAsync();
        }

        public async Task<TblServiceUserTree> GetTblServiceUserTree(Guid serviceUserTreeID)
        {
            return await _context.TblServiceUserTrees.Where(x => x.Id.Equals(serviceUserTreeID)).FirstOrDefaultAsync();
        }

        public async Task<TblUser> getTblUserByID(Guid userID)
        {
            return await _context.TblUsers.Where(x => x.Id.Equals(userID)).FirstOrDefaultAsync();
        }

        public async Task<TblUser> getTblUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<TblUserTree> getUserTreeByID(Guid userTreeID)
        {
            return await _context.TblUserTrees.Where(x => x.Id.Equals(userTreeID)).FirstOrDefaultAsync();
        }

        public async Task<bool> removeAll(Guid userID)
        {
            var result = await _context.TblServiceOrders.Where(x=>x.UserId==userID).ToListAsync();
            foreach (var r in result)
            {
                _context.Remove(r);
                await _context.SaveChangesAsync();  
            }
            return true;

        }

        public async Task<bool> updateServiceOrder(TblServiceOrder entity)
        {
            _context.TblServiceOrders.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateServiceUserTree(TblServiceUserTree entity)
        {
            _context.TblServiceUserTrees.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
