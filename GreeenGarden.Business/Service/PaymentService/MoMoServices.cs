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
        public async Task<ResultModel> DepositPaymentMoMo(MoMoDepositModel moMoDepositModel)
        {
            ResultModel resultModel = new ResultModel();
            double amount = 0;
            string base64OrderString = "";
            MoMoOrderModel moMoOrderModel = new();
            if (moMoDepositModel.OrderType.ToLower().Trim().Equals("rent"))
            {
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(moMoDepositModel.OrderId);
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
                moMoOrderModel.OrderId = moMoDepositModel.OrderId;
                moMoOrderModel.PayAmount = amount;
                moMoOrderModel.OrderType = moMoDepositModel.OrderType;
                var orderJsonStringRaw = JsonConvert.SerializeObject(moMoOrderModel, Formatting.Indented,
                    jsonSerializerSettings);
                var orderTextBytes = System.Text.Encoding.UTF8.GetBytes(orderJsonStringRaw);
                 base64OrderString = Convert.ToBase64String(orderTextBytes);
            }
            else if (moMoDepositModel.OrderType.ToLower().Trim().Equals("sale"))
            {
                TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(moMoDepositModel.OrderId);
                amount = (double)tblSaleOrder.Deposit;
                if (tblSaleOrder == null)
                {
                    resultModel.Code = 400;
                    resultModel.IsSuccess = false;
                    resultModel.Message = "Rent order Id invalid.";
                    return resultModel;
                }
                if (tblSaleOrder.Status.Equals(Status.READY))
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
                moMoOrderModel.OrderId = moMoDepositModel.OrderId;
                moMoOrderModel.PayAmount = amount;
                var orderJsonStringRaw = JsonConvert.SerializeObject(moMoOrderModel, Formatting.Indented,
                    jsonSerializerSettings);
                var orderTextBytes = System.Text.Encoding.UTF8.GetBytes(orderJsonStringRaw);
                base64OrderString = Convert.ToBase64String(orderTextBytes);
            }
            else
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "service order not yet available.";
                return resultModel;
            }

            List<string> secrets = SecretService.SecretService.GetPaymentSecrets();
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = secrets[0];
            string accessKey = secrets[1];
            string serectkey = secrets[2];
            string orderInfo = "GreenGarden Payment";
            string redirectUrl = "https://ggarden.shop/thanks";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-deposit-payment-momo";
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
            Console.WriteLine("Response from MoMo: " + responseFromMomo.ToString());
            JObject resJSON = JObject.Parse(responseFromMomo);
            resultModel.Code = 200;
            resultModel.IsSuccess = true;
            resultModel.Message = "Create sale payment success.";
            resultModel.Data = resJSON;
            return resultModel;
        }

        public async Task<ResultModel> DepositPaymentCash(MoMoDepositModel moMoDepositModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (moMoDepositModel.OrderType.Trim().ToLower().Equals("rent"))
                {
                    TblRentOrder tblRentOrder = await _rentOrderRepo.Get(moMoDepositModel.OrderId);
                    if (tblRentOrder.Status.Equals(Status.READY))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Order deposit has been paid.";
                        return result;
                    }
                    ResultModel updateDeposit = await _rentOrderRepo.UpdateRentOrderDeposit(moMoDepositModel.OrderId);
                    if(updateDeposit.IsSuccess == true)
                    {

                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Update order deposit success.";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Update order deposit failed.";
                        return result;
                    }
                }else if (moMoDepositModel.OrderType.Trim().ToLower().Equals("sale"))
                {
                    TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(moMoDepositModel.OrderId);
                    if (tblSaleOrder.Status.Equals(Status.READY))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Order deposit has been paid.";
                        return result;
                    }
                    ResultModel updateDeposit = await _saleOrderRepo.UpdateSaleOrderDeposit(moMoDepositModel.OrderId);
                    if (updateDeposit.IsSuccess == true)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Update order deposit success.";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Update order deposit failed.";
                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Update service deposit not yet available.";
                    return result;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> OrderPaymentCash(MoMoPaymentModel moMoPaymentModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (moMoPaymentModel.OrderType.Trim().ToLower().Equals("rent"))
                {
                    TblRentOrder tblRentOrder = await _rentOrderRepo.Get(moMoPaymentModel.OrderId);
                    if (tblRentOrder.Status.Equals(Status.PAID))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Order is fully paid.";
                        return result;
                    }
                    if (moMoPaymentModel.Amount < 1000 || moMoPaymentModel.Amount > tblRentOrder.RemainMoney)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Payment amount must be greater than 1000 and less than Remain amount.";
                        return result;
                    }
                    ResultModel updateDeposit = await _rentOrderRepo.UpdateRentOrderRemain(moMoPaymentModel.OrderId, moMoPaymentModel.Amount);
                    if (updateDeposit.IsSuccess == true)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Update order payment success.";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Update order payment failed.";
                        return result;
                    }
                }
                else if (moMoPaymentModel.OrderType.Trim().ToLower().Equals("sale"))
                {
                    TblSaleOrder tblSaleOrder = await _saleOrderRepo.Get(moMoPaymentModel.OrderId);
                    if (tblSaleOrder.Status.Equals(Status.COMPLETED))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Order is fully paid.";
                        return result;
                    }
                    if (moMoPaymentModel.Amount < 1000 || moMoPaymentModel.Amount > tblSaleOrder.RemainMoney)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Payment amount must be greater than 1000 and less than Remain amount.";
                        return result;
                    }
                    ResultModel updateDeposit = await _saleOrderRepo.UpdateSaleOrderRemain(moMoPaymentModel.OrderId, moMoPaymentModel.Amount);
                    if (updateDeposit.IsSuccess == true)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Update order payment success.";
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Update order payment failed.";
                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Update service payment not yet available.";
                    return result;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<bool> ProcessDepositPaymentMoMo(MoMoResponseModel moMoResponseModel)
        {
            try
            {
                var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
                var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
                var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
                if (orderModel != null && moMoResponseModel.resultCode == 0)
                {
                    if (orderModel.OrderType.Trim().ToLower().Equals("rent"))
                    {
                        ResultModel updateDeposit = await _rentOrderRepo.UpdateRentOrderDeposit(orderModel.OrderId);
                        return updateDeposit.IsSuccess;
                    }
                    else if (orderModel.OrderType.Trim().ToLower().Equals("sale"))
                    {
                        ResultModel updateDeposit = await _saleOrderRepo.UpdateSaleOrderDeposit(orderModel.OrderId);
                        return updateDeposit.IsSuccess;
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
            catch
            {
                return false;
            }
        }
    }
}
