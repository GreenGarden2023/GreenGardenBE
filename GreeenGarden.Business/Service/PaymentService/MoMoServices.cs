using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Repositories.OrderRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GreeenGarden.Business.Service.PaymentService
{
    public class MoMoServices : IMoMoService
    {
        private readonly IOrderRepo _orderRepo;
        public MoMoServices(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<string> CreateOrderPayment(Guid payOrderID)
        {
            var order = await _orderRepo.Get(payOrderID);

            JsonSerializerSettings jsonSerializerSettings = new()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            MoMoOrderModel moMoOrderModel = new MoMoOrderModel
            {
                OrderId = payOrderID,
                PayAmount = order.TotalPrice
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
            string redirectUrl = "https://greengarden2023.azurewebsites.net/swagger/index.html";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/swagger/index.html";
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
            Console.WriteLine(responseFromMomo);
            return responseFromMomo;

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
