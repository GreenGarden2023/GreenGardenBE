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

        public async Task<listOrderShowModel> GetListOrder(Guid userID)
        {
            var user = await _context.TblUsers.Where(x=>x.Id== userID).FirstOrDefaultAsync();
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            var result = new listOrderShowModel();

            result.user = new user()
            {
                userID = userID,
                userName = user.UserName,
                fullName = user.FullName,
                address = user.Address,
                phone = user.Phone,
                mail = user.Mail,

            };
            result.orderShowModels = new List<orderShowModel>();
            foreach (var order in listOrder) {
                var listAddendum = await _context.TblAddendums.Where(x=>x.OrderId == order.Id).ToListAsync();

                var orderShow = new orderShowModel();
                orderShow.orderID = order.Id;
                orderShow.totalPrice= order.TotalPrice;
                orderShow.createDate= order.CreateDate;
                orderShow.status= order.Status;
                orderShow.isForRent = (bool)order.IsForRent;
                orderShow.addendumShowModels = new List<addendumShowModel>();

                foreach (var addendum in listAddendum)
                {
                    var listAddendumProductItems = await _context.TblAddendumProductItems.Where(x => x.AddendumId == addendum.Id).ToListAsync();

                    var addendumShow = new addendumShowModel();
                    addendumShow.addendumID= addendum.Id;
                    addendumShow.endDateRent= addendum.EndDateRent;
                    addendumShow.startDateRent= addendum.StartDateRent;
                    addendumShow.deposit= addendum.Deposit;
                    addendumShow.transportFee= addendum.TransportFee;
                    addendumShow.reducedMoney= addendum.ReducedMoney;
                    addendumShow.totalPrice= addendum.TotalPrice;
                    addendumShow.status= addendum.Status;
                    addendumShow.remainMoney= addendum.RemainMoney;
                    addendumShow.address= addendum.Address;
                    addendumShow.addendumProductItemShowModels = new List<addendumProductItemShowModel>();
                    foreach (var addendumProductItems in listAddendumProductItems)
                    {
                        var sizeProductItem = await _context.TblSizeProductItems.Where(x => x.Id == addendumProductItems.SizeProductItemId).FirstOrDefaultAsync();
                        var productItem = await _context.TblProductItems.Where(x => x.Id == sizeProductItem.ProductItemId).FirstOrDefaultAsync();
                        var size = await _context.TblSizes.Where(x => x.Id == sizeProductItem.SizeId).FirstOrDefaultAsync();


                        var addendumProductItemShow = new addendumProductItemShowModel();
                        addendumProductItemShow.addendumProductItemID = addendumProductItems.Id;
                        addendumProductItemShow.sizeProductItemPrice = addendumProductItems.SizeProductItemPrice;
                        addendumProductItemShow.Quantity = addendumProductItems.Quantity;
                        addendumProductItemShow.sizeProductItems = new sizeProductItemShowModel()
                        {
                            productName = productItem.Name,
                            sizeName = size.Name,
                            sizeProductItemID =addendumProductItems.SizeProductItemId,
                        };
                        addendumShow.addendumProductItemShowModels.Add(addendumProductItemShow);
                    }
                    orderShow.addendumShowModels.Add(addendumShow);
                }
                result.orderShowModels.Add(orderShow);
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
