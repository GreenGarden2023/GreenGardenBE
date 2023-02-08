using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> checkWholesaleProduct(Guid subProductId, int quantity);
        Task<ResultModel> checkRetailProduct(Guid productItemId);
    }
}
