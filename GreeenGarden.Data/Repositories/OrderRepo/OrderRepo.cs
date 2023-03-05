using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.AddendumModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public class OrderRepo : Repository<TblOrder>, IOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public OrderRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TblSizeProductItem> GetSizeProductItem(Guid sizeProductItemID)
        {
           return await _context.TblSizeProductItems.Where(x=>x.Id.Equals(sizeProductItemID)).FirstOrDefaultAsync();
        }

        public async Task<AdddendumResponseModel> getDetailAddendum(Guid AddendumId)
        {
            var result = new AdddendumResponseModel();
            var addendum = await _context.TblAddendums.Where(x => x.Id == AddendumId).FirstAsync();
            var addendumProductItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == AddendumId).ToListAsync();
            result.id = addendum.Id;
            result.transportFee = addendum.TransportFee;
            result.startDateRent = addendum.StartDateRent;
            result.endDateRent = addendum.EndDateRent;
            result.deposit = addendum.Deposit;
            result.reducedMoney = addendum.ReducedMoney;
            result.totalPrice = addendum.TotalPrice;
            result.status = addendum.Status;
            result.orderID = addendum.OrderId;
            result.remainMoney = addendum.RemainMoney;
            result.sizeProductItems = new List<addendumProductItemResponseModel>();
            foreach (var item in addendumProductItem)
            {
                var Items = new addendumProductItemResponseModel()
                {
                    sizeProductItemID = item.SizeProductItemId,
                    sizeProductItemPrice = item.SizeProductItemPrice,
                    quantity = item.Quantity
                };
                result.sizeProductItems.Add(Items);
            }
            return result;
        }

        public async Task<List<listAddendumResponse>> getListAddendum(Guid OrderId)
        {
            var result = new List<listAddendumResponse>();
            var addendumResponse = new listAddendumResponse();
            var addendum = await _context.TblAddendums.Where(x => x.OrderId == OrderId).ToListAsync();
            foreach (var item in addendum)
            {
                addendumResponse.id = item.Id;
                addendumResponse.orderID = item.OrderId;
                addendumResponse.startDateRent = item.StartDateRent;
                addendumResponse.endDateRent = item.EndDateRent;
                addendumResponse.status = item.Status;
                addendumResponse.deposit = item.Deposit;
                addendumResponse.totalPrice = item.TotalPrice;
                addendumResponse.remainMoney = item.RemainMoney;
                addendumResponse.productItems = new List<addendumProductItemResponseModel>();
                var addendumItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == item.Id).ToListAsync();
                foreach (var i in addendumItem)
                {
                    var addProItem = new addendumProductItemResponseModel()
                    {
                        sizeProductItemID = i.SizeProductItemId,
                        sizeProductItemPrice = i.SizeProductItemPrice,
                        quantity = i.Quantity,
                    };
                    addendumResponse.productItems.Add(addProItem);
                }
                result.Add(addendumResponse);
            }
            return result;
        } //cmt

        public async Task<List<orderShowModel>> GetListOrder(Guid userID)
        {
            var result = new List<orderShowModel>();
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            foreach (var order in listOrder) {
                var listAddendum = await _context.TblAddendums.Where(x=>x.OrderId == order.Id).ToListAsync();
                foreach (var addendum in listAddendum)
                {

                }
            }
            return result;
        }

        public async Task<TblOrder> GetOrder(Guid OrderId)
        {
            return await _context.TblOrders.Where(x => x.Id == OrderId).FirstOrDefaultAsync();
        }

        public async Task<TblUser> GetUser(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<TblRole> GetRole(Guid roleID)
        {
            return await _context.TblRoles.Where(x => x.Id == roleID).FirstOrDefaultAsync();
        }

        public async Task<TblAddendum> insertAddendum(TblAddendum entities)
        {
            await _context.TblAddendums.AddAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TblAddendumProductItem> insertAddendumProductItem(TblAddendumProductItem entities)
        {
            await _context.TblAddendumProductItems.AddAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<bool> minusQuantitySizeProductItem(Guid sizeProductItemID, int quantity)
        {
            var result = await _context.TblSizeProductItems.Where(x => x.Id.Equals(sizeProductItemID)).FirstOrDefaultAsync();
            if (result.Quantity < quantity) return false;
            result.Quantity -= quantity;
            _context.TblSizeProductItems.Update(result);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderPayment(Guid orderID)
        {
            try
            {
                var order = await _context.TblOrders.Where(x => x.Id.Equals(orderID)).FirstOrDefaultAsync();
                if(order != null)
                {
                    order.Status = Status.COMPLETED;
                    _context.TblOrders.Update(order);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> updateStatusAddendum(Guid addendumID, string status)
        {
            var addendum = await _context.TblAddendums.Where(x=>x.Id.Equals(addendumID)).FirstOrDefaultAsync();
            addendum.Status = status;
            _context.TblAddendums.Update(addendum);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> updateStatusOrder(Guid orderID, string status)
        {
            var order = await _context.TblOrders.Where(x=>x.Id== orderID).FirstOrDefaultAsync();
            order.Status = status;
            _context.TblOrders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<managerOrderModel>> getListOrderByManager()
        {
            var listOrder = await _context.TblOrders.ToListAsync();
            var result = new List<managerOrderModel>();
            foreach (var order in listOrder)
            {
                var user = await _context.TblUsers.Where(x=>x.Id== order.UserId).FirstOrDefaultAsync();
                var userTemp = new user()
                {
                    userID = user.Id,
                    address = user.Address,
                    favorite = user.Favorite,
                    fullName = user.FullName,
                    mail = user.Mail,
                    phone = user.Phone,
                    userName = user.UserName
                };
                var orderTemp = new managerOrderModel()
                {
                    orderId = order.Id,
                    totalPrice = order.TotalPrice,
                    createDate = order.CreateDate,
                    status = order.Status,
                    isForRent = order.IsForRent,
                    user = userTemp
                };
                result.Add(orderTemp);
            }
            return result;
        }
    }
}
