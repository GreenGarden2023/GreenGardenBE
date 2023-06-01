using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace GreeenGarden.Data.Repositories.RentOrderGroupRepo
{
    public class RentOrderGroupRepo : Repository<TblRentOrderGroup>, IRentOrderGroupRepo
    {
        private readonly GreenGardenDbContext _context;
        public RentOrderGroupRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Page<TblRentOrderGroup>> GetAllRentOrderGroup(PaginationRequestModel paginationRequestModel)
        {
            Page<TblRentOrderGroup> tblRentOrderGroups = await _context.TblRentOrderGroups.OrderByDescending(x=>x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return tblRentOrderGroups;
        }

        public async Task<Page<TblRentOrderGroup>> GetRentOrderGroup(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblRentOrderGroup> tblRentOrderGroups = await _context.TblRentOrderGroups.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return tblRentOrderGroups;
        }


        public async Task<ResultModel> UpdateRentOrderGroup(Guid rentOrderGroupID, double newRentOrderAmount)
        {
            ResultModel result = new();
            TblRentOrderGroup tblRentOrderGroup = await _context.TblRentOrderGroups.Where(x => x.Id.Equals(rentOrderGroupID)).FirstOrDefaultAsync();
            tblRentOrderGroup.NumberOfOrders += 1;
            tblRentOrderGroup.GroupTotalAmount += newRentOrderAmount;
            _ = _context.Update(tblRentOrderGroup);
            _ = await _context.SaveChangesAsync();
            result.Code = 200;
            result.IsSuccess = true;
            result.Message = "Update rent order group success.";
            return result;
        }
    }
}

