using GreeenGarden.Data.Entities;
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

        public async Task<TblAddendum> GetAddendum(Guid AddendumId)
        {
            return await _context.TblAddendums.Where(x => x.Id == AddendumId).FirstAsync();
        }

        public async Task<AdddendumResponseModel> getDetailAddendum(Guid AddendumId)
        {
            var result = new AdddendumResponseModel();
            var addendum = await _context.TblAddendums.Where(x => x.Id == AddendumId).FirstAsync();
            var addendumProductItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == AddendumId).ToListAsync();
            result.Id = addendum.Id;
            result.TransportFee = addendum.TransportFee;
            result.StartDateRent = addendum.StartDateRent;
            result.EndDateRent = addendum.EndDateRent;
            result.Deposit = addendum.Deposit;
            result.ReducedMoney = addendum.ReducedMoney;
            result.TotalPrice = addendum.TotalPrice;
            result.Status = addendum.Status;
            result.OrderID = addendum.OrderId;
            result.RemainMoney = addendum.RemainMoney;
            result.ProductItems = new List<addendumProductItemResponseModel>();
            foreach (var item in addendumProductItem)
            {
                var Items = new addendumProductItemResponseModel()
                {
                    ProductItemID = item.ProductItemId,
                    ProductItemPrice = item.ProductItemPrice,
                    Quantity = item.Quantity
                };
                result.ProductItems.Add(Items);
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
                addendumResponse.Id = item.Id;
                addendumResponse.OrderID = item.OrderId;
                addendumResponse.StartDateRent = item.StartDateRent;
                addendumResponse.EndDateRent = item.EndDateRent;
                addendumResponse.Status = item.Status;
                addendumResponse.Deposit = item.Deposit;
                addendumResponse.TotalPrice = item.TotalPrice;
                addendumResponse.RemainMoney = item.RemainMoney;
                addendumResponse.ProductItems = new List<addendumProductItemResponseModel>();
                var addendumItem = await _context.TblAddendumProductItems.Where(x => x.AddendumId == item.Id).ToListAsync();
                foreach (var i in addendumItem)
                {
                    var addProItem = new addendumProductItemResponseModel()
                    {
                        ProductItemID = i.ProductItemId,
                        ProductItemPrice = i.ProductItemPrice,
                        Quantity = i.Quantity,
                    };
                    addendumResponse.ProductItems.Add(addProItem);
                }
                result.Add(addendumResponse);
            }
            return result;
        }

        public async Task<List<listOrderResponseModel>> GetListOrder(Guid userID)
        {
            var result = new List<listOrderResponseModel>();
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            foreach (var order in listOrder)
            {
                var orderResponseModel = new listOrderResponseModel()
                {
                    CreateDate = order.CreateDate,
                    OrderId = order.Id,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    VoucherID = order.VoucherId
                };
                result.Add(orderResponseModel);
            }
            return result;
        }

        public async Task<TblOrder> GetOrder(Guid OrderId)
        {
            return await _context.TblOrders.Where(x => x.Id == OrderId).FirstOrDefaultAsync();
        }

        public async Task<TblProductItem> getProductToCompare(Guid productId)
        {
            return await _context.TblProductItems.Where(x => x.Id == productId).FirstAsync();
        }

        public async Task<TblUser> GetUser(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<TblUser> getUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName == username).FirstAsync();
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

    }
}
