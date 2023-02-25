using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.OrderRepo;

namespace GreeenGarden.Business.Service.OrderService
{
    public class OrderService : IOrderService
    {
        //private readonly IMapper _mapper;
        private readonly IOrderRepo _orderRepo;
        public OrderService(/*IMapper mapper,*/ IOrderRepo orderRepo)
        {
            //_mapper = mapper;
            _orderRepo = orderRepo;
        }

        public async Task<ResultModel> checkRetailProduct(Guid productItemId)
        {
            var result = new ResultModel();
            /*try
            {
                bool check = _orderRepo.checkRetailProduct(productItemId);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = check;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }*/
            return result;
        }

        public async Task<ResultModel> checkWholesaleProduct(Guid subProductId, int quantity)
        {
            var result = new ResultModel();
            try
            {
                bool check =  _orderRepo.checkWholesaleProduct(subProductId, quantity);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = check;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> createOrder(string token, Guid productItem, Guid productSize, int quantity, DateTime startDate, DateTime endDate)
        {
            var result = new ResultModel();
            /* try
             {
                 // check token

                 if (productItem != null)
                 {
                    bool check = _orderRepo.checkRetailProduct(productItem);
                     if (!check)
                     {
                         result.IsSuccess = false;
                         result.Message = "dont have ProductItem";
                         return result;
                     }
                 }
                 if (productSize !=null)
                 {
                     bool check = _orderRepo.checkWholesaleProduct(productSize, quantity);
                     if (!check)
                     {
                         result.IsSuccess = false;
                         result.Message = "Product size dont enought item";
                         return result;
                     }
                 }
                 if (startDate < DateTime.Now || startDate >= endDate)
                 {
                     result.IsSuccess = false;
                     result.Message = "startDate invalid";
                     return result;
                 }

             }
             catch (Exception e)
             {
                 result.IsSuccess = false;
                 result.Code = 400;
                 result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
             }*/
            return result;
        }
    }
}
