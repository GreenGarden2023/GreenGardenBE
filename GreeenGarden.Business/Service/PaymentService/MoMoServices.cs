using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GreeenGarden.Business.Service.PaymentService
{
    public class MoMoServices : IMoMoService
    {



        public MoMoServices() {}

        public async Task<string> CreatePayment()
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOFH0Y20221003";
            string accessKey = "YAX7OxslhYkTXzbK";
            string serectkey = "jrQjul5BxdOMI7kHvIYAWCka1XIXXF7M";
            string orderInfo = "test";
            string redirectUrl = "https://calm-plant-066944600.2.azurestaticapps.net";
            string ipnUrl = "https://calm-plant-066944600.2.azurestaticapps.net";
            string requestType = "captureWallet";
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "exData";
            long amount = 20000;

            //Before sign HMAC SHA256 signature
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
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            Console.WriteLine("Signature = " + signature);

            //build body json request
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
            Console.WriteLine(responseFromMomo);
            return responseFromMomo;
            
        }

       

    }
}
