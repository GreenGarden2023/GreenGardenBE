using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public class OrderRepo : IOrderRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public OrderRepo(/*IMapper mapper,*/ GreenGardenDbContext context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public bool checkRetailProduct(Guid productItemId)
        {
            var productItemToCheck = _context.TblProductItems.Where(x=>x.Id == productItemId && x.Status == "Active").FirstOrDefault();
            if (productItemToCheck != null) return true;
            return false;
        }

        public bool checkWholesaleProduct(Guid subProductId, int quantity)
        {
            var subProductToCheck = _context.TblSubProducts.Where(x=>x.Id== subProductId && x.Quantity >= quantity).FirstOrDefault();
            if (subProductToCheck != null) return true;
            return false;
        }
    }
}
