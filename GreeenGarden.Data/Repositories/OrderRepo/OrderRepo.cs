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

        public async Task<orderDetail> getDetailOrder(Guid OrderId, int flag, Guid? addendumID)//flag = 1: default, 2:getAddendumByID
        {
            var result = new orderDetail();
            var order = await _context.TblOrders.Where(x=>x.Id.Equals(OrderId)).FirstOrDefaultAsync();
            var user = await _context.TblUsers.Where(x => x.Id == order.UserId).FirstOrDefaultAsync();
            result.user = new user()
            {
                userID = user.Id,
                userName = user.UserName,
                fullName = user.FullName,
                address = user.Address,
                phone = user.Phone,
                mail = user.Mail,
            };
            result.order = new orderShowModel()
            {
                orderID = order.Id,
                totalPrice = order.TotalPrice,
                createDate = order.CreateDate,
                status = order.Status,
                isForRent = (bool)order.IsForRent,
                addendums = new List<addendumShowModel>(),
            };
            var listAddendum = new List<TblAddendum>();
            if (flag == 1) listAddendum = await _context.TblAddendums.Where(x => x.OrderId == order.Id).ToListAsync();
            if (flag == 2)
            {
                var addendum = await _context.TblAddendums.Where(x => x.Id == addendumID && x.OrderId == order.Id).FirstOrDefaultAsync();
                listAddendum.Add(addendum);
            }
            foreach (var addendum in listAddendum)
            {
                var listAddendumProductItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == addendum.Id).ToListAsync(); 

                var addendumShow = new addendumShowModel();
                addendumShow.addendumID = addendum.Id;
                addendumShow.endDateRent = addendum.EndDateRent;
                addendumShow.startDateRent = addendum.StartDateRent;
                addendumShow.deposit = addendum.Deposit;
                addendumShow.transportFee = addendum.TransportFee;
                addendumShow.reducedMoney = addendum.ReducedMoney;
                addendumShow.totalPrice = addendum.TotalPrice;
                addendumShow.status = addendum.Status;
                addendumShow.remainMoney = addendum.RemainMoney;
                addendumShow.address = addendum.Address;
                addendumShow.addendumProductItems = new List<addendumProductItemShowModel>();
                foreach (var addendumProductItem in listAddendumProductItem)
                {
                    var sizeProductItem = await _context.TblSizeProductItems.Where(x => x.Id == addendumProductItem.SizeProductItemId).FirstOrDefaultAsync();
                    var productItem = await _context.TblProductItems.Where(x => x.Id == sizeProductItem.ProductItemId).FirstOrDefaultAsync();
                    var size = await _context.TblSizes.Where(x => x.Id == sizeProductItem.SizeId).FirstOrDefaultAsync();
                    var listImg = await _context.TblImages.Where(x => x.SizeProductItemId == sizeProductItem.Id).ToListAsync();
                    List<string> listImgUrl = new List<string>();
                    foreach (var img in listImg)
                    {
                        listImgUrl.Add(img.ImageUrl);
                    }

                    var addendumProductItemShow = new addendumProductItemShowModel();
                    addendumProductItemShow.addendumProductItemID = addendumProductItem.Id;
                    addendumProductItemShow.sizeProductItemPrice = addendumProductItem.SizeProductItemPrice;
                    addendumProductItemShow.Quantity = addendumProductItem.Quantity;

                    addendumProductItemShow.sizeProductItems = new sizeProductItemShowModel()
                    {
                        productName = productItem.Name,
                        sizeName = size.Name,
                        sizeProductItemID = addendumProductItem.SizeProductItemId,
                        imgUrl = listImgUrl
                    };
                    addendumShow.addendumProductItems.Add(addendumProductItemShow);
                }
                result.order.addendums.Add(addendumShow);
            }
            return result;


        } //cmt

        public async Task<listOrder> GetListOrder(Guid userID)
        {
            var user = await _context.TblUsers.Where(x=>x.Id== userID).FirstOrDefaultAsync();
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            var result = new listOrder();

            result.user = new user()
            {
                userID = userID,
                userName = user.UserName,
                fullName = user.FullName,
                address = user.Address,
                phone = user.Phone,
                mail = user.Mail,

            };
            result.orders = new List<orderShowModel>();
            foreach (var order in listOrder) {
                var listAddendum = await _context.TblAddendums.Where(x=>x.OrderId == order.Id).ToListAsync();

                var orderShow = new orderShowModel();
                orderShow.orderID = order.Id;
                orderShow.totalPrice= order.TotalPrice;
                orderShow.createDate= order.CreateDate;
                orderShow.status= order.Status;
                orderShow.isForRent = (bool)order.IsForRent;
                orderShow.addendums = new List<addendumShowModel>();

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
                    addendumShow.addendumProductItems = new List<addendumProductItemShowModel>();
                    foreach (var addendumProductItems in listAddendumProductItems)
                    {
                        var sizeProductItem = await _context.TblSizeProductItems.Where(x => x.Id == addendumProductItems.SizeProductItemId).FirstOrDefaultAsync();
                        var productItem = await _context.TblProductItems.Where(x => x.Id == sizeProductItem.ProductItemId).FirstOrDefaultAsync();
                        var size = await _context.TblSizes.Where(x => x.Id == sizeProductItem.SizeId).FirstOrDefaultAsync();
                        var img = await _context.TblImages.Where(x => x.SizeProductItemId == sizeProductItem.Id).ToListAsync();
                        var listImgUrl = new List<string>();
                        foreach (var image in img) {
                            listImgUrl.Add(image.ImageUrl);
                        }

                        var addendumProductItemShow = new addendumProductItemShowModel();
                        addendumProductItemShow.addendumProductItemID = addendumProductItems.Id;
                        addendumProductItemShow.sizeProductItemPrice = addendumProductItems.SizeProductItemPrice;
                        addendumProductItemShow.Quantity = addendumProductItems.Quantity;

                        addendumProductItemShow.sizeProductItems = new sizeProductItemShowModel()
                        {
                            productName = productItem.Name,
                            sizeName = size.Name,
                            sizeProductItemID =addendumProductItems.SizeProductItemId,
                            imgUrl = listImgUrl
                        };
                        addendumShow.addendumProductItems.Add(addendumProductItemShow);
                    }
                    orderShow.addendums.Add(addendumShow);
                }
                result.orders.Add(orderShow);
            }
            return result;
        }

        public async Task<TblOrder> GetOrder(Guid? OrderId, Guid? AddendumID)
        {
            if (OrderId != null) return await _context.TblOrders.Where(x => x.Id == OrderId).FirstOrDefaultAsync();
            if (AddendumID != null)
            {
                var addendum =  await _context.TblAddendums.Where(x=>x.Id== AddendumID).FirstOrDefaultAsync();
                return await _context.TblOrders.Where(x => x.Id == addendum.OrderId).FirstOrDefaultAsync();
            }
            return null;
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

        public async Task<bool> removeCart(Guid userID)
        {
            var cart = await _context.TblCarts.Where(x=>x.UserId==userID).FirstOrDefaultAsync();
            if (cart == null) return true;
            var cartDetail = await _context.TblCartDetails.Where(x => x.CartId == cart.Id).ToListAsync();
            if (cartDetail == null) return true;
            foreach (var item in cartDetail)
            {
                _context.TblCartDetails.Remove(item);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<TblUser>> GetListUser()
        {
            return await _context.TblUsers.ToListAsync();
        }

        public async Task<bool> removeOrder(Guid userID)
        {
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            foreach (var order in listOrder)
            {
                var listAddendum = await _context.TblAddendums.Where(x=>x.OrderId==order.Id).ToListAsync();
                foreach (var addedum in listAddendum)
                {
                    var listAddendumProductItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == addedum.Id).ToListAsync();
                    foreach (var addendumProductItem in listAddendumProductItem)
                    {
                        _context.TblAddendumProductItems.Remove(addendumProductItem);
                        await _context.SaveChangesAsync();
                    }
                    _context.TblAddendums.Remove(addedum);
                    await _context.SaveChangesAsync();
                }
                _context.TblOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<TblAddendum> getLastAddendum(Guid orderId)
        {
            return await _context.TblAddendums.Where(x => x.OrderId == orderId).LastOrDefaultAsync();
        }
    }
}
