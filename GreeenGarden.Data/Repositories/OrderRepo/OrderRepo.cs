using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.AddendumModel;
using GreeenGarden.Data.Models.OrderModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public class OrderRepo : Repository<TblOrder>, IOrderRepo
    {
        private readonly GreenGardenDbContext _context;
        public OrderRepo( GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TblSizeProductItem> GetSizeProductItem(Guid sizeProductItemID)
        {
           return await _context.TblSizeProductItems.Where(x=>x.Id.Equals(sizeProductItemID)).FirstOrDefaultAsync();
        }

        /*public async Task<TblAddendum> GetAddendum(Guid AddendumId)
        {
            return await _context.TblAddendums.Where(x=>x.Id == AddendumId).FirstAsync();
        }*/

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

        public async Task<List<listOrderResponseModel>> GetListOrder(Guid userID)
        {
            var result = new List<listOrderResponseModel>();
            var listOrder = await _context.TblOrders.Where(x => x.UserId == userID).ToListAsync();
            foreach (var order in listOrder)
            {
                var orderResponseModel = new listOrderResponseModel()
                {
                    createDate = order.CreateDate,
                    orderId = order.Id,
                    status = order.Status,
                    totalPrice = order.TotalPrice,
                    voucherID = order.VoucherId
                };
                result.Add(orderResponseModel);
            }
            return result;
        }

        /*public async Task<TblOrder> GetOrder(Guid OrderId)
        {
            return await _context.TblOrders.Where(x => x.Id == OrderId).FirstOrDefaultAsync();
        }*/

        public async Task<TblUser> GetUser(string username)
        {
            return await _context.TblUsers.Where(x => x.UserName == username).FirstOrDefaultAsync();
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
    }
}
