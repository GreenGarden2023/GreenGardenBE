using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.OrderService
{
    public class OrderService : IOrderService
    {
        //private readonly IMapper _mapper;
        private readonly IOrderRepo _orderRepo;
        public OrderService(/*IMapper mapper,*/ IOrderRepo orderRepo)
        {
            //_mapper = mapper;
            _orderRepo= orderRepo;
        }

        public async Task<ResultModel> checkRetailProduct(Guid productItemId)
        {
            var result = new ResultModel();
            try
            {
                bool check = _orderRepo.checkRetailProduct(productItemId);
                result.IsSuccess = true;
                result.Code = 200;
                result.Result = check;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> checkWholesaleProduct(Guid subProductId, int quantity)
        {
            var result = new ResultModel();
            try
            {
                bool check = _orderRepo.checkWholesaleProduct(subProductId, quantity);
                result.IsSuccess = true;
                result.Code = 200;
                result.Result = check;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
