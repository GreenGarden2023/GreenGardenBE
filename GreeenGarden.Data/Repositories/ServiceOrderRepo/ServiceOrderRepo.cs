using AutoMapper;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> changeStatusServiceOrder(ServiceOrderChangeStatusModel model)
        {
            TblServiceOrder? result = await _context.TblServiceOrders.Where(x => x.Id.Equals(model.ServiceOrderId)).FirstOrDefaultAsync();
            result.Status = model.Status;
            return true;

        }

        public async Task<bool> createServiceOrder(TblServiceOrder entity)
        {
            _ = await _context.AddAsync(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceUserTree(TblServiceUserTree entity)
        {
            _ = _context.TblServiceUserTrees.Remove(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetailServiceOrderResModel> GetDetailServiceOrder(Guid SerOrderID)
        {
            DetailServiceOrderResModel result = new();

            TblServiceOrder? soData = await _context.TblServiceOrders.Where(x => x.Id.Equals(SerOrderID)).FirstOrDefaultAsync();
            TblUser? userData = await _context.TblUsers.Where(x => x.Id.Equals(soData.UserId)).FirstOrDefaultAsync();
            result.User = _mapper.Map<UserResModel>(userData);
            result.ServiceOrder = _mapper.Map<ServiceOrderResModel>(soData);

            TblUser? techData = await _context.TblUsers.Where(x => x.Id.Equals(soData.TechnicianId)).FirstOrDefaultAsync();
            result.ServiceOrder.Technician = _mapper.Map<UserResModel>(techData);

            TblService? sData = await _context.TblServices.Where(x => x.Id.Equals(soData.ServiceId)).FirstOrDefaultAsync();
            result.ServiceOrder.Service = _mapper.Map<ServiceResModel>(sData);

            result.ServiceOrder.Service.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
            List<TblServiceUserTree> listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(sData.Id)).ToListAsync();
            foreach (TblServiceUserTree? sut in listSut)
            {
                ServiceUserTreeRespModel sutRecord = new();
                sutRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);

                TblUserTree? utData = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                UserTreeResModel utRecord = new();
                utRecord = _mapper.Map<UserTreeResModel>(utData);
                utRecord.ImageUrl = await getImgUrlByUserTreeID(utData.Id);

                sutRecord.UserTree = utRecord;
                result.ServiceOrder.Service.ServiceUserTrees.Add(sutRecord);
            }
            return result;
        }

        public async Task<List<string>> getImgUrlByUserTreeID(Guid userTreeID)
        {
            List<string> result = new();
            List<TblImage> listImg = await _context.TblImages.Where(x => x.UserTreeId.Equals(userTreeID)).ToListAsync();
            foreach (TblImage? i in listImg)
            {
                result.Add(i.ImageUrl);
            }
            return result;
        }

        public async Task<ListServiceOrderResModel> GetListServiceOrder(TblUser user)
        {
            ListServiceOrderResModel result = new();
            List<TblServiceOrder> listServiceOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            result.User = _mapper.Map<UserResModel>(user);
            result.ServiceOrder = new List<ServiceOrderResModel>();
            foreach (TblServiceOrder? so in listServiceOrder)
            {
                ServiceOrderResModel ServiceOrderRes = new();
                TblService? s = await _context.TblServices.Where(x => x.Id.Equals(so.ServiceId)).FirstOrDefaultAsync();
                List<TblServiceUserTree> listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();

                ServiceOrderResModel ServiceRes = _mapper.Map<ServiceOrderResModel>(s);

                ServiceOrderRes.Service.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
                foreach (TblServiceUserTree? sut in listSut)
                {
                    ServiceUserTreeRespModel ServiceUserTreesRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);
                    ServiceOrderRes.Service.ServiceUserTrees.Add(ServiceUserTreesRecord);
                }
                result.ServiceOrder.Add(ServiceOrderRes);
            }
            return result;
        } // chưa test

        public async Task<List<ServiceOrderResManagerModel>> GetListServiceOrderByManager()
        {
            List<ServiceOrderResManagerModel> result = new();
            List<TblServiceOrder> listSo = await _context.TblServiceOrders.ToListAsync();
            foreach (TblServiceOrder? so in listSo)
            {
                TblUser? tech = await _context.TblUsers.Where(x => x.Id.Equals(so.TechnicianId)).FirstOrDefaultAsync();
                TblUser? user = await _context.TblUsers.Where(x => x.Id.Equals(so.UserId)).FirstOrDefaultAsync();
                TblService? s = await _context.TblServices.Where(x => x.Id.Equals(so.ServiceId)).FirstOrDefaultAsync();
                List<TblServiceUserTree> listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                UserResModel techRecord = _mapper.Map<UserResModel>(tech);
                UserResModel userRecord = _mapper.Map<UserResModel>(user);
                ServiceResModel serRecord = _mapper.Map<ServiceResModel>(s);
                serRecord.ServiceUserTrees = new List<ServiceUserTreeRespModel>();
                foreach (TblServiceUserTree? sut in listSut)
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

        public async Task<List<DetailServiceOrderResModel>> GetListServiceOrderByTechnician(Guid techID)
        {
            List<DetailServiceOrderResModel> listResult = new();

            List<TblServiceOrder> listSO = await _context.TblServiceOrders.Where(x => x.TechnicianId.Equals(techID)).ToListAsync();
            foreach (TblServiceOrder? so in listSO)
            {
                DetailServiceOrderResModel result = new();
                TblUser? user = await _context.TblUsers.Where(x => x.Id.Equals(so.UserId)).FirstOrDefaultAsync();

                result.User = _mapper.Map<UserResModel>(user);
                ServiceOrderResModel soRecord = new();
                soRecord = _mapper.Map<ServiceOrderResModel>(so);

                ServiceResModel serviceRecord = new();
                TblService? s = await _context.TblServices.Where(x => x.Id.Equals(so.ServiceId)).FirstOrDefaultAsync();
                serviceRecord = _mapper.Map<ServiceResModel>(s);
                serviceRecord.ServiceUserTrees = new List<ServiceUserTreeRespModel>();

                List<TblServiceUserTree> listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                foreach (TblServiceUserTree? sut in listSut)
                {
                    ServiceUserTreeRespModel sutRecord = new();
                    sutRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);

                    TblUserTree? ut = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                    UserTreeResModel utRecord = new();
                    utRecord = _mapper.Map<UserTreeResModel>(ut);
                    utRecord.ImageUrl = await getImgUrlByUserTreeID(ut.Id);
                    sutRecord.UserTree = utRecord;

                    serviceRecord.ServiceUserTrees.Add(sutRecord);
                }
                soRecord.Service = serviceRecord;

                result.ServiceOrder = soRecord;
                listResult.Add(result);
            }


            return listResult;
        }

        public async Task<List<TblServiceUserTree>> GetListTblServiceUserTreeByServiceID(Guid serviceID)
        {
            return await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(serviceID)).ToListAsync();
        }

        public async Task<List<UserResModel>> getListTecnician()
        {
            List<UserResModel> result = new();
            TblRole? role = await _context.TblRoles.Where(x => x.RoleName.Equals("Technician")).FirstOrDefaultAsync();
            List<TblUser> listTech = await _context.TblUsers.Where(x => x.RoleId.Equals(role.Id)).ToListAsync();
            foreach (TblUser? tech in listTech)
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
            List<TblServiceOrder> result = await _context.TblServiceOrders.Where(x => x.UserId == userID).ToListAsync();
            foreach (TblServiceOrder? r in result)
            {
                _ = _context.Remove(r);
                _ = await _context.SaveChangesAsync();
            }
            return true;

        }

        public async Task<bool> updateServiceOrder(TblServiceOrder entity)
        {
            _ = _context.TblServiceOrders.Update(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateServiceUserTree(TblServiceUserTree entity)
        {
            _ = _context.TblServiceUserTrees.Update(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }
    }
}
