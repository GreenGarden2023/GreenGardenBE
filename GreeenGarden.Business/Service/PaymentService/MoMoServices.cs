using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GreeenGarden.Business.Service.PaymentService
{
    public class MoMoServices : IMoMoService
    {
        private readonly IRentOrderRepo _rentOrderRepo;
        private readonly ISaleOrderRepo _saleOrderRepo;
        public MoMoServices(IRentOrderRepo rentOrderRepo, ISaleOrderRepo saleOrderRepo)
        {
            _rentOrderRepo = rentOrderRepo;
            _saleOrderRepo = saleOrderRepo;
        }
        public async Task<ResultModel> CreateDepositPaymentMoMo(Guid orderID, string orderType)
        {
            ResultModel resultModel = new ResultModel();
            double amount = 0;
            string base64OrderString = "";
            MoMoOrderModel moMoOrderModel = new();
            if (orderType.ToLower().Trim().Equals("rent"))
            {
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(orderID);
                 amount = (double)tblRentOrder.Deposit;
                if (tblRentOrder == null)
                {
                    resultModel.Code = 400;
                    resultModel.IsSuccess = false;
                    resultModel.Message = "Rent order Id invalid.";
                    return resultModel;
                }
                if (tblRentOrder.Status.Equals(Status.READY))
                {
                    resultModel.Code = 400;
                    resultModel.IsSuccess = false;
                    resultModel.Message = "Rent order deposit is paid.";
                    return resultModel;
                }

                JsonSerializerSettings jsonSerializerSettings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                moMoOrderModel.OrderId = orderID;
                moMoOrderModel.PayAmount = amount;
                var orderJsonStringRaw = JsonConvert.SerializeObject(moMoOrderModel, Formatting.Indented,
                    jsonSerializerSettings);
                var orderTextBytes = System.Text.Encoding.UTF8.GetBytes(orderJsonStringRaw);
                 base64OrderString = Convert.ToBase64String(orderTextBytes);
            }

            List<string> secrets = SecretService.SecretService.GetPaymentSecrets();
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = secrets[0];
            string accessKey = secrets[1];
            string serectkey = secrets[2];
            string orderInfo = "GreenGarden Payment";
            string redirectUrl = "https://ggarden.shop/thanks";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-sale-payment-reponse";
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
            resultModel.Message = "Create sale payment success.";
            resultModel.Data = resJSON;
            return resultModel;
        }
    }
}
