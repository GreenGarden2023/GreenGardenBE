using AutoMapper;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceRepo
{
    public class ServiceRepo : Repository<TblService>, IServiceRepo
    {
        private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ServiceRepo(IMapper mapper, GreenGardenDbContext context) : base(context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> ChangeStatusService(Guid serviceID, string status)
        {
            TblService? service = await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
            service.Status = status;
            _ = _context.TblServices.Update(service);
            _ = await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteServiceUserTree(TblServiceUserTree entities)
        {
            _ = _context.TblServiceUserTrees.Remove(entities);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetailServiceByCustomerResModel> GetDetailServiceByCustomer(Guid serviceID)
        {
            DetailServiceByCustomerResModel result = new();
            TblService? service = await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
            TblUser? user = await _context.TblUsers.Where(x => x.Id.Equals(service.UserId)).FirstOrDefaultAsync();

            UserResModel userRecord = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                Mail = user.Mail,
            };
            result.User = userRecord;
            result.Services = new ServiceForListResModel
            {
                Id = service.Id,
                StartDate = service.StartDate,
                EndDate = service.EndDate,
                Mail = service.Mail,
                Phone = service.Phone,
                Address = service.Address,
                Status = service.Status,
                UserTrees = new List<ServiceUserTreeRespModel>()
            };

            List<TblServiceUserTree> listSut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(service.Id)).ToListAsync();
            foreach (TblServiceUserTree? sut in listSut)
            {
                TblUserTree? ut = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                UserTreeResModel utRecord = new()
                {
                    Id = ut.Id,
                    TreeName = ut.TreeName,
                    Description = ut.Description,
                    Quantity = ut.Quantity,
                    Status = ut.Status,
                    ImageUrl = await getImgUrlByUserTreeID(ut.Id)
                };

                ServiceUserTreeRespModel sutRecord = new()
                {
                    Id = sut.Id,
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
            List<string> result = new();
            List<TblImage> listImg = await _context.TblImages.Where(x => x.UserTreeId.Equals(userTreeID)).ToListAsync();
            foreach (TblImage? i in listImg)
            {
                result.Add(i.ImageUrl);
            }
            return result;
        }

        public async Task<ListServiceByCustomerResModel> GetListServiceByCustomer(TblUser user)
        {
            ListServiceByCustomerResModel result = new()
            {
                User = _mapper.Map<UserResModel>(user),
                //result.User = userRecord;
                Services = new List<ServiceForListResModel>()
            };
            List<TblService> listService = await _context.TblServices.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
            foreach (TblService? s in listService)
            {
                ServiceForListResModel serviceRecord = new()
                {
                    Id = s.Id,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Mail = s.Mail,
                    Phone = s.Phone,
                    Address = s.Address,
                    Status = s.Status,
                    UserTrees = new List<ServiceUserTreeRespModel>()
                };

                List<TblServiceUserTree> listServiceUserTree = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                foreach (TblServiceUserTree? sut in listServiceUserTree)
                {
                    TblUserTree? ut = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                    UserTreeResModel utRecord = new()
                    {
                        Id = ut.Id,
                        TreeName = ut.TreeName,
                        Description = ut.Description,
                        Quantity = ut.Quantity,
                        Status = ut.Status,
                        ImageUrl = await getImgUrlByUserTreeID(ut.Id)
                    };

                    ServiceUserTreeRespModel sutRecord = new()
                    {
                        Id = sut.Id,
                        Quantity = sut.Quantity,
                        Price = sut.Price,
                        UserTree = utRecord
                    };

                    serviceRecord.UserTrees.Add(sutRecord);
                }
                result.Services.Add(serviceRecord);
            }
            return result;
        }

        public async Task<TblService> GetTblService(Guid serviceID)
        {
            return await _context.TblServices.Where(x => x.Id.Equals(serviceID)).FirstOrDefaultAsync();
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
            _ = await _context.TblServiceUserTrees.AddAsync(entities);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateService(TblService entities)
        {
            _ = _context.TblServices.Update(entities);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateServiceUserTree(TblServiceUserTree entities)
        {
            _ = _context.TblServiceUserTrees.Update(entities);
            _ = await _context.SaveChangesAsync();
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

        public async Task<List<ServiceByManagerResModel>> GetListServiceByManager()
        {
            List<ServiceByManagerResModel> result = new();
            List<TblService> listSer = await _context.TblServices.ToListAsync();
            foreach (TblService? s in listSer)
            {
                ServiceByManagerResModel sRecord = _mapper.Map<ServiceByManagerResModel>(s);
                TblUser? user = await _context.TblUsers.Where(x => x.Id.Equals(s.UserId)).FirstOrDefaultAsync();
                sRecord.User = _mapper.Map<UserResModel>(user);

                sRecord.UserTrees = new List<ServiceUserTreeRespModel>();
                List<TblServiceUserTree> listUut = await _context.TblServiceUserTrees.Where(x => x.ServiceId.Equals(s.Id)).ToListAsync();
                foreach (TblServiceUserTree? sut in listUut)
                {
                    ServiceUserTreeRespModel sutRecord = new();
                    sutRecord = _mapper.Map<ServiceUserTreeRespModel>(sut);
                    TblUserTree? ut = await _context.TblUserTrees.Where(x => x.Id.Equals(sut.UserTreeId)).FirstOrDefaultAsync();
                    sutRecord.UserTree = _mapper.Map<UserTreeResModel>(ut);
                    sutRecord.UserTree.ImageUrl = new List<string>();
                    sutRecord.UserTree.ImageUrl = await getImgUrlByUserTreeID(ut.Id);

                    sRecord.UserTrees.Add(sutRecord);
                }
                result.Add(sRecord);
            }
            return result;
        }
    }
}
