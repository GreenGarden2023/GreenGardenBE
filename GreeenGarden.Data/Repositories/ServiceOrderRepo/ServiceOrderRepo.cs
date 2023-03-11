using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public class ServiceOrderRepo : Repository<TblServiceOrder>, IServiceOrderRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public ServiceOrderRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }


        public async Task<List<TblRequestDetail>> getListRequestDetail( Guid requestID)
        {
            return await _context.TblRequestDetails.Where(x=>x.RequestId.Equals(requestID)).ToListAsync();
        }

        public async Task<ServiceOrderResponseModel> getServiceOrder(Guid userID)
        {
            var ServiceOrderRecord = new ServiceOrderResponseModel();

            var user = await _context.TblUsers.Where(x => x.Id.Equals(userID)).FirstOrDefaultAsync();
            ServiceOrderRecord.User = new UserResponseModel()
            {
                Fullname = user.UserName,
                Mail = user.Mail,
                Phone= user.Phone,  
                UserID= userID,
                Username = user.UserName,
            };
            ServiceOrderRecord.ServiceOrders = new List<ServiceOrderShowModel>();

            // với 1 thằng user sẽ lấy được 1 list serviceOrder
            var listSerOrder = new List<TblServiceOrder>();
            var request = await _context.TblRequests.Where(x=>x.UserId.Equals(userID)).ToListAsync();
            foreach (var i in request)
            {
                var serviceOrder = await _context.TblServiceOrders.Where(x => x.RequestId.Equals(i.Id)).FirstOrDefaultAsync();
                var technician = await _context.TblUsers.Where(x => x.Id.Equals(serviceOrder.TechnicianId)).FirstOrDefaultAsync();
                var techRecord = new TechicianModel()
                {
                    Fullname= technician.UserName,
                    Mail= technician.Mail,
                    Phone= technician.Phone,
                    TechnicianID = technician.Id,
                    Username = technician.UserName,
                };
                var ServiceOrderShow = new ServiceOrderShowModel();

                ServiceOrderShow.CreateDate = serviceOrder.CreateDate;
                ServiceOrderShow.Deposit = serviceOrder.Deposit;
                ServiceOrderShow.RewardPointGain = serviceOrder.RewardPointGain;
                ServiceOrderShow.RewardPointUsed = serviceOrder.RewardPointUsed;
                ServiceOrderShow.ServiceEndDate = serviceOrder.ServiceEndDate;
                ServiceOrderShow.ServiceOrderID = serviceOrder.Id;
                ServiceOrderShow.ServiceStartDate = serviceOrder.ServiceStartDate;
                ServiceOrderShow.Status = serviceOrder.Status;
                ServiceOrderShow.Technician = techRecord;
                ServiceOrderShow.requestDetails = new List<RequestDetailModel>();

                var requestDetail = await _context.TblRequestDetails.Where(x => x.ServiceOrderId.Equals(serviceOrder.Id)).ToListAsync();
                foreach (var item in requestDetail)
                {
                    var requestRecord = new RequestDetailModel();
                    requestRecord.Price = item.Price;
                    requestRecord.Quantity = item.Quantity;
                    requestRecord.Description = item.Description;
                    requestRecord.TreeName= item.TreeName;
                    requestRecord.ID = item.Id;
                    ServiceOrderShow.requestDetails.Add(requestRecord);
                }
                ServiceOrderRecord.ServiceOrders.Add(ServiceOrderShow);
            }
            return ServiceOrderRecord;
        }

        public async Task<List<TechicianModel>> getTechnician()
        {
            var role = await _context.TblRoles.Where(x => x.RoleName.Equals("Technician")).FirstOrDefaultAsync();
            var tech = await _context.TblUsers.Where(x => x.RoleId.Equals(role.Id)).ToListAsync();
            var result = new List<TechicianModel>();    
            foreach (var i in tech)
            {
                var techModel = new TechicianModel();
                techModel.TechnicianID = i.Id;
                techModel.Username = i.UserName;
                techModel.Fullname = i.FullName;
                techModel.Phone = i.Phone;
                techModel.Mail = i.Mail;
                result.Add(techModel);

            }
            return result;
        }

        public async Task<TblUser> GetUser(string username)
        {
            return await _context.TblUsers.Where(x=>x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<bool> removeRequestDetail(TblRequestDetail entities)
        {
            try
            {
                _context.TblRequestDetails.Remove(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> updateRequestDetail(TblRequestDetail entities)
        {
            try
            {
                _context.TblRequestDetails.Update(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
