using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.RewardRepo;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace GreeenGarden.Data.Repositories.SaleOrderRepo
{
    public class SaleOrderRepo : Repository<TblSaleOrder>, ISaleOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IRewardRepo _rewardRepo;
        public SaleOrderRepo(GreenGardenDbContext context, IRewardRepo rewardRepo) : base(context)
        {
            _context = context;
            _rewardRepo = rewardRepo;
        }

        public async Task<ResultModel> UpdateSaleOrderStatus(Guid SaleOrderID, string status, string? username)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(SaleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
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
                result.Message = "Update order status success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update order status failed.";
                return result;
            }
        }

        public async Task<Page<TblSaleOrder>> GetSaleOrders(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblSaleOrder> listTblOrder = await _context.TblSaleOrders.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<Page<TblSaleOrder>> GetAllSaleOrders(PaginationRequestModel paginationRequestModel)
        {
            Page<TblSaleOrder> listTblOrder = await _context.TblSaleOrders.PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<ResultModel> UpdateSaleOrderDeposit(Guid SaleOrderID)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(SaleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainMoney -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update deposit sucess.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update deposit sucess.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateSaleOrderRemain(Guid saleOrderID, double amount)
        {
            ResultModel result = new();
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.Id.Equals(saleOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.RemainMoney -= amount;
                if (order.RemainMoney == 0)
                {
                    order.Status = Status.PAID;
                    _ = await _rewardRepo.AddUserRewardPointByUserID((Guid)order.UserId, (int)order.RewardPointGain);
                }
               
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Cancel rent order sucess.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Cancel rent order failed.";
                return result;
            }
        }

        public async Task<bool> CheckOrderCode(string code)
        {
            TblSaleOrder order = await _context.TblSaleOrders.Where(x => x.OrderCode.Equals(code)).FirstOrDefaultAsync();
            return order != null;
        }

        public async Task<Page<TblSaleOrder>> GetSaleOrderByOrderCode(OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            try
            {
                var result = new Page<TblSaleOrder>();
                var listSaleOrder = await GetSaleOrderBy(model.Phone, model.Status, model.OrderCode);

                var listResultPaging = listSaleOrder.Skip((pagingModel.curPage - 1) * pagingModel.pageSize).Take(pagingModel.pageSize);

                result.PageSize = pagingModel.pageSize;
                result.CurrentPage = pagingModel.curPage;
                result.RecordCount = listSaleOrder.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RecordCount / result.PageSize);

                result.Results = listResultPaging.ToList();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<TblSaleOrder>> GetSaleOrderBy(string phone, string status, string orderCode)
        {
            try
            {
                var result = new List<TblSaleOrder>();
                if (orderCode != null && orderCode.Trim() != "")
                {
                    var saleOrder = await _context.TblSaleOrders.Where(x => x.OrderCode.Equals(orderCode)).FirstOrDefaultAsync();
                    if (saleOrder != null) result.Add(saleOrder);
                    return result;
                }
                if (status != null && status.Trim() != "" && phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var saleOrderByPhone = await _context.TblSaleOrders.Where(x => x.UserId.Equals(user.Id) && x.Status.Equals(status)).ToListAsync();
                    if (saleOrderByPhone.Any() == true)
                    {
                        foreach (var s in saleOrderByPhone)
                        {
                            result.Add(s);
                        }
                    }
                    return result.OrderBy(x => x.CreateDate).ToList();

                }
                if (status != null && status.Trim() != "")
                {
                    var saleOrder = await _context.TblSaleOrders.Where(x => x.Status.Equals(status)).ToListAsync();
                    if (saleOrder.Any() == true)
                    {
                        foreach (var r in saleOrder)
                        {
                            result.Add(r);
                        }
                    }
                }
                if (phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var saleOrders = await _context.TblSaleOrders.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
                    if (saleOrders.Any() == true)
                    {
                        foreach (var r in saleOrders)
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

        public async Task<bool> UpdateSaleOrder(TblSaleOrder entity)
        {
            _context.TblSaleOrders.Update(entity); 
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

