using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.AddendumRepo;
using GreeenGarden.Data.Repositories.OrderRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GreeenGarden.Business.Service.PaymentService
{
    public class MoMoServices : IMoMoService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IAddendumRepo _addendumRepo;
        public MoMoServices(IOrderRepo orderRepo, IAddendumRepo addendumRepo)
        {
            _orderRepo = orderRepo;
            _addendumRepo = addendumRepo;
        }

        public async Task<ResultModel> CreateAddendumPayment(Guid addendumId, double amount)
        {
            ResultModel resultModel = new ResultModel();
            TblAddendum tblAddendum = await _addendumRepo.Get(addendumId);
            if(tblAddendum == null)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum Id invalid.";
                return resultModel;
            }
            if (tblAddendum.RemainMoney < amount)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "The pay amount exceed the remain amount";
                return resultModel;
            }
            
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            MoMoOrderModel moMoOrderModel = new MoMoOrderModel
            {
                OrderId = addendumId,
                PayAmount = amount
            };
            var orderJsonStringRaw = JsonConvert.SerializeObject(moMoOrderModel, Formatting.Indented,
                jsonSerializerSettings);
            var orderTextBytes = System.Text.Encoding.UTF8.GetBytes(orderJsonStringRaw);
            var base64OrderString = Convert.ToBase64String(orderTextBytes);

            List<string> secrets = SecretService.SecretService.GetPaymentSecrets();
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = secrets[0];
            string accessKey = secrets[1];
            string serectkey = secrets[2];
            string orderInfo = "GreenGarden Payment";
            string redirectUrl = "https://ggarden.shop";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-addendum-payment-reponse";
            string requestType = "captureWallet";
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = base64OrderString;

            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            Console.WriteLine("rawHash = " + rawHash);

            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);
            Console.WriteLine("Signature = " + signature);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            Console.WriteLine("Json request to MoMo: " + message.ToString());
            string responseFromMomo = await Task.FromResult(PaymentRequest.sendPaymentRequest(endpoint, message.ToString()));
            JObject resJSON = JObject.Parse(responseFromMomo);
            resultModel.Code = 200;
            resultModel.IsSuccess = true;
            resultModel.Message = "Create payment success.";
            resultModel.Data = resJSON;
            return resultModel;
        }

        public async Task<ResultModel> CreateOrderPayment(Guid payOrderID)
        {
            ResultModel resultModel = new ResultModel();
            var order = await _orderRepo.Get(payOrderID);
            if(order == null)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Order Id invalid.";
                return resultModel;
            }
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            MoMoOrderModel moMoOrderModel = new MoMoOrderModel
            {
                OrderId = payOrderID,
                PayAmount = (double)order.TotalPrice
            };
            var orderJsonStringRaw = JsonConvert.SerializeObject(moMoOrderModel, Formatting.Indented,
                jsonSerializerSettings);
            var orderTextBytes = System.Text.Encoding.UTF8.GetBytes(orderJsonStringRaw);
            var base64OrderString = Convert.ToBase64String(orderTextBytes);

            List<string> secrets = SecretService.SecretService.GetPaymentSecrets();
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = secrets[0];
            string accessKey = secrets[1];
            string serectkey = secrets[2];
            string orderInfo = "GreenGarden Payment";
            string redirectUrl = "https://ggarden.shop";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-order-payment-reponse";
            string requestType = "captureWallet";
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = base64OrderString;

            string rawHash = "accessKey=" + accessKey +
                "&amount=" + order.TotalPrice +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            Console.WriteLine("rawHash = " + rawHash);

            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);
            Console.WriteLine("Signature = " + signature);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", order.TotalPrice },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            Console.WriteLine("Json request to MoMo: " + message.ToString());
            string responseFromMomo = await Task.FromResult(PaymentRequest.sendPaymentRequest(endpoint, message.ToString()));
            JObject resJSON = JObject.Parse(responseFromMomo);
            resultModel.Code = 200;
            resultModel.IsSuccess = true;
            resultModel.Message = "Create payment success.";
            return resultModel;

        }

        public async Task<bool> ProcessAddendumPayment(MoMoResponseModel moMoResponseModel)
        {
            var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
            var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
            var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
            if (orderModel != null && moMoResponseModel.resultCode == 0)
            {
                var updateAddendum = await _addendumRepo.UpdateAddendumPayment(orderModel.OrderId, orderModel.PayAmount);
                if(updateAddendum.IsSuccess == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ProcessOrderPayment(MoMoResponseModel moMoResponseModel)
        {
            var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
            var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
            var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
            if (orderModel != null && moMoResponseModel.resultCode == 0)
            {
                var updateOrder = await _orderRepo.UpdateOrderPayment(orderModel.OrderId);
                if(updateOrder == true)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
