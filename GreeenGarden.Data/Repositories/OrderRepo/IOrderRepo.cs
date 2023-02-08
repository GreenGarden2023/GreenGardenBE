using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public interface IOrderRepo
    {
        public Boolean checkWholesaleProduct(Guid subProductId, int quantity);
        public Boolean checkRetailProduct(Guid productItemId);
    }
}
