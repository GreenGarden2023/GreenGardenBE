using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.RewardRepo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Repositories.RentOrderRepo
{
    public class RentOrderRepo : Repository<TblRentOrder>, IRentOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IRewardRepo _rewardRepo; 
        public RentOrderRepo(GreenGardenDbContext context, IRewardRepo rewardRepo) : base(context)
        {
            _context = context;
            _rewardRepo = rewardRepo;   
        }

        public async Task<ResultModel> UpdateRentOrderStatus(Guid RentOrderID, string status, string username = null)
        {

            ResultModel result = new();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(RentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                if (status.Trim().ToLower().Equals(Status.COMPLETED))
                {
                    _ = await _rewardRepo.AddUserRewardPointByUserID((Guid)order.UserId, (int)order.RewardPointGain);
                }
                if (status.Trim().ToLower().Equals(Status.CANCEL))
                {
                    var user = await _context.TblUsers.Where(x=>x.UserName.Equals(username)).FirstOrDefaultAsync();
                    order.CancelBy = user.Id;
                }
                order.Status = status.Trim().ToLower();
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update rent order status success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update rent order status failed.";
                return result;
            }
        }

        public async Task<List<TblRentOrder>> GetRentOrders(Guid userID)
        {
            List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.UserId.Equals(userID)).ToListAsync();
            return listTblOrder;
        }

        public async Task<List<TblRentOrder>?> GetRentOrdersByGroup(Guid rentOrderGroupID)
        {
            List<TblRentOrder> listTblOrder = await _context.TblRentOrders.Where(x => x.RentOrderGroupId.Equals(rentOrderGroupID)).ToListAsync();
            return listTblOrder.Any() ? listTblOrder : null;
        }

        public async Task<ResultModel> UpdateRentOrderDeposit(Guid rentOrderID)
        {
            ResultModel result = new();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(rentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainMoney -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = order.Deposit;
                result.Message = "Update deposit success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update deposit failed.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateRentOrderRemain(Guid rentOrderID, double amount)
        {
            ResultModel result = new();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(rentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {

                order.RemainMoney -= amount;
                if (order.RemainMoney == 0)
                {
                    order.Status = Status.PAID;
                }
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update rent order success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update rent order failed.";
                return result;
            }
        }

        public async Task<bool> CheckOrderCode(string Code)
        {
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.OrderCode.Equals(Code)).FirstOrDefaultAsync();
            return order != null;

        }

        public async Task<Page<TblRentOrder>> SearchRentOrder(OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            try
            {
                var result = new Page<TblRentOrder>();
                var listResult = await GetRentOrderBy(model.Phone, model.Status, model.OrderCode);

                var listResultPaging = listResult.Skip((pagingModel.curPage - 1) * pagingModel.pageSize).Take(pagingModel.pageSize);

                result.PageSize = pagingModel.pageSize;
                result.CurrentPage = pagingModel.curPage;
                result.RecordCount = listResult.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RecordCount / result.PageSize);

                result.Results = listResultPaging.ToList();

                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Page<TblRentOrder>> GetRentOrderByDate(DateTime fromDate, DateTime toDate, PaginationRequestModel pagingModel)
        {
            return await _context.TblRentOrders.Where(x => x.EndDateRent >= fromDate && x.EndDateRent <= toDate).OrderByDescending(y => y.EndDateRent).PaginateAsync(pagingModel.curPage, pagingModel.pageSize);
        }
        
        public async Task<List<TblRentOrder>> GetRentOrderBy(string phone, string status, string orderCode)
        {
            try
            {
                var result = new List<TblRentOrder>();
                if (orderCode != null && orderCode.Trim() != "")
                {
                    var rentOrder = await _context.TblRentOrders.Where(x => x.OrderCode.Equals(orderCode)).FirstOrDefaultAsync();
                    if (rentOrder != null) result.Add(rentOrder);
                    return result;
                }
                if (status != null && status.Trim() != "" && phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var rentOrderByPhone = await _context.TblRentOrders.Where(x => x.UserId.Equals(user.Id) && x.Status.Equals(status)).ToListAsync();
                    if (rentOrderByPhone.Any() == true)
                    {
                        foreach (var r in rentOrderByPhone)
                        {
                            result.Add(r);
                        }
                    }
                    return result.OrderBy(x => x.CreateDate).ToList();

                }
                if (status != null && status.Trim() != "")
                {
                    var rentOrder = await _context.TblRentOrders.Where(x => x.Status.Equals(status)).ToListAsync();
                    if (rentOrder.Any() == true)
                    {
                        foreach (var r in rentOrder)
                        {
                            result.Add(r);
                        }
                    }
                }
                if (phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var rentOrder = await _context.TblRentOrders.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
                    if (rentOrder.Any() == true)
                    {
                        foreach (var r in rentOrder)
                        {
                            result.Add(r);
                        }
                    }
                }

                return result.OrderBy(x => x.CreateDate).ToList();
            }
            catch (Exception)
            {
                return null;
            }            
        }

        public async Task<bool> UpdateRentOrder(TblRentOrder entity)
        {
            _context.TblRentOrders.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ResultModel> UpdateRentOrderContractUrl(Guid rentOrderID, string contracURL)
        {
            ResultModel result = new();
            TblRentOrder order = await _context.TblRentOrders.Where(x => x.Id.Equals(rentOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.ContractUrl = contracURL;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = order.Deposit;
                result.Message = "Update deposit success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update deposit failed.";
                return result;
            }
        }
    }
}

