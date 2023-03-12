using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.RequestModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.RequestRepo
{
    public class RequestRepo : Repository<TblRequest>, IRequestRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public RequestRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<RequestResponseModel> GetListRequest(Guid UserID)
        {
            var result = new RequestResponseModel();
            var user = await _context.TblUsers.Where(x=>x.Id.Equals(UserID)).FirstOrDefaultAsync();
            result.user.UserID = user.Id;
            result.user.Username = user.UserName;
            result.user.Fullname = user.FullName;
            result.user.Phone = user.Phone;
            result.user.Mail = user.Mail;
            result.request = new List<RequestResponse>();

            var listRequest = await _context.TblRequests.Where(x => x.UserId == user.Id).ToListAsync();
            foreach (var i in listRequest)
            {
                var requestRecord = new RequestResponse();
                requestRecord.RequestID = i.Id;
                requestRecord.UserID = i.UserId ;
                requestRecord.Address = i.Address ;
                requestRecord.Phone = i.Phone ;
                requestRecord.CreateDate = i.CreateDate ;
                requestRecord.Status = i.Status ;
                requestRecord.RequestDetail = new List<RequestDetailResponse>();

                var listRequestDetail = await _context.TblRequestDetails.Where(x=>x.RequestId== i.Id).ToListAsync();
                foreach (var j in listRequestDetail)
                {
                    var requestDetailRecord = new RequestDetailResponse();
                    requestDetailRecord.RequestDetailID = j.Id;
                    requestDetailRecord.TreeName = j.TreeName;
                    requestDetailRecord.Quantity = j.Quantity;
                    requestDetailRecord.Description = j.Description;
                    requestDetailRecord.ImageUrl = await getImgUrlByRequestDetailID(j.Id);
                    requestRecord.RequestDetail.Add(requestDetailRecord);
                }
                result.request.Add(requestRecord);               
            }
            return result;
        }

        public async Task<TblUser> GetUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertImage(TblImage entities)
        {
            await _context.TblImages.AddAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InsertRequestDetail(TblRequestDetail entities)
        {
            await _context.TblRequestDetails.AddAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> getImgUrlByRequestDetailID(Guid requestDetailID)
        {
            var result = new List<string>();
            var listImg = await _context.TblImages.Where(x => x.RequestDetailId.Equals(requestDetailID)).ToListAsync();
            foreach ( var img in listImg)
            {
                result.Add(img.ImageUrl);
            }
            return result;
        }

        public async Task<bool> changeStatus(RequestUpdateStatusModel model)
        {
            var result = await _context.TblRequests.Where(x => x.Id.Equals(model.RequestID)).FirstOrDefaultAsync();
            result.Status = model.Status;
            _context.TblRequests.Update(result);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
