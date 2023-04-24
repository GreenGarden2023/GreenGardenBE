using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public class ServiceOrderRepo : Repository<TblServiceOrder>, IServiceOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IServiceRepo _serviceRepo;
        private readonly IRewardRepo _rewardRepo;
        public ServiceOrderRepo(GreenGardenDbContext context, IServiceRepo serviceRepo, IRewardRepo rewardRepo) : base(context)
        {
            _context = context;
            _serviceRepo = serviceRepo;
            _rewardRepo = rewardRepo;   
        }

        public async Task<bool> CheckOrderCode(string Code)
        {
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.OrderCode.Equals(Code)).FirstOrDefaultAsync();
            return order != null;
        }

        public async Task<bool> CompleteServiceOrder(Guid serviceOrderID)
        {
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.Id.Equals(serviceOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.COMPLETED;
                _ = await _rewardRepo.AddUserRewardPointByUserID((Guid)order.UserId, (int)order.RewardPointGain);
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                TblService tblService = await _context.TblServices.Where(x => x.Id.Equals(order.ServiceId)).FirstOrDefaultAsync();
                tblService.Status = ServiceStatus.COMPLETED;
                _ = _context.Update(tblService);
                _ = await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Page<TblServiceOrder>> GetAllServiceOrders(PaginationRequestModel paginationRequestModel)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.OrderByDescending(x=>x.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<TblServiceOrder> GetServiceOrderByOrderCode(string orderCode)
        {
            return await _context.TblServiceOrders.Where(x => x.OrderCode.Equals(orderCode)).FirstOrDefaultAsync();
        }

        public async Task<TblServiceOrder> GetServiceOrderByServiceID(Guid serviceId)
        {
            TblServiceOrder order = await _context.TblServiceOrders.OrderByDescending(y => y.CreateDate).Where(x => x.ServiceId.Equals(serviceId)).FirstOrDefaultAsync();
            return order ?? null;
        }

        public async Task<Page<TblServiceOrder>> GetServiceOrderByTechnician(PaginationRequestModel paginationRequestModel, Guid technicianID)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.Where(x => x.TechnicianId.Equals(technicianID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }

        public async Task<Page<TblServiceOrder>> GetServiceOrders(PaginationRequestModel paginationRequestModel, Guid userID)
        {
            Page<TblServiceOrder> listTblOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(userID)).OrderByDescending(y => y.CreateDate).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            return listTblOrder;
        }


        public async Task<bool> UpdateServiceOrder(TblServiceOrder entity)
        {
            _ = _context.TblServiceOrders.Update(entity);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ResultModel> UpdateServiceOrderDeposit(Guid serviceOrderID)
        {
            ResultModel result = new();
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.Id.Equals(serviceOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = Status.READY;
                order.RemainAmount -= order.Deposit;
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update service order sucess.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update service order failed.";
                return result;
            }
        }

        public async Task<ResultModel> UpdateServiceOrderRemain(Guid serviceOrderID, double amount)
        {
            ResultModel result = new();
            TblServiceOrder order = await _context.TblServiceOrders.Where(x => x.Id.Equals(serviceOrderID)).FirstOrDefaultAsync();
            if (order != null)
            {

                order.RemainAmount -= amount;
                if (order.RemainAmount == 0)
                {
                    order.Status = Status.PAID;
                }
                _ = _context.Update(order);
                _ = await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Update service order success.";
                return result;
            }
            else
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update service order failed.";
                return result;
            }
        }
        public async Task<Page<TblServiceOrder>> SearchServiceOrder(OrderFilterModel model, PaginationRequestModel pagingModel)
        {
            try
            {
                var result = new Page<TblServiceOrder>();
                var listServiceOrder = await GetSaleOrderBy(model.Phone, model.Status, model.OrderCode);

                var listResultPaging = listServiceOrder.Skip((pagingModel.curPage - 1) * pagingModel.pageSize).Take(pagingModel.pageSize);

                result.PageSize = pagingModel.pageSize;
                result.CurrentPage = pagingModel.curPage;
                result.RecordCount = listServiceOrder.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RecordCount / result.PageSize);

                result.Results = listResultPaging.ToList();

                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<TblServiceOrder>> GetSaleOrderBy(string phone, string status, string orderCode)
        {
            try
            {
                var result = new List<TblServiceOrder>();
                if (orderCode != null && orderCode.Trim() != "")
                {
                    var serviceOrder = await _context.TblServiceOrders.Where(x => x.OrderCode.Equals(orderCode)).FirstOrDefaultAsync();
                    if (serviceOrder != null) result.Add(serviceOrder);
                    return result;
                }
                if (status != null && status.Trim() != "" && phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var serviceOrderByPhone = await _context.TblServiceOrders.Where(x => x.UserId.Equals(user.Id) && x.Status.Equals(status)).ToListAsync();
                    if (serviceOrderByPhone.Any() == true)
                    {
                        foreach (var s in serviceOrderByPhone)
                        {
                            result.Add(s);
                        }
                    }
                    return result.OrderBy(x => x.CreateDate).ToList();

                }
                if (status != null && status.Trim() != "")
                {
                    var serviceOrder = await _context.TblServiceOrders.Where(x => x.Status.Equals(status)).ToListAsync();
                    if (serviceOrder.Any() == true)
                    {
                        foreach (var r in serviceOrder)
                        {
                            result.Add(r);
                        }
                    }
                }
                if (phone != null && phone.Trim() != "")
                {
                    var user = await _context.TblUsers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();
                    var serviceOrder = await _context.TblServiceOrders.Where(x => x.UserId.Equals(user.Id)).ToListAsync();
                    if (serviceOrder.Any() == true)
                    {
                        foreach (var r in serviceOrder)
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
    }
}

