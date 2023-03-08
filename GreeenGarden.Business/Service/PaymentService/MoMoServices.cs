using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.AddendumRepo;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Data.Repositories.TransactionRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace GreeenGarden.Business.Service.PaymentService
{
    public class MoMoServices : IMoMoService
    {
        private readonly IAddendumRepo _addendumRepo;
        private readonly ITransactionRepo _transactionRepo;
        public MoMoServices(IOrderRepo orderRepo, IAddendumRepo addendumRepo, ITransactionRepo transactionRepo )
        {
            _addendumRepo = addendumRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<ResultModel> CreateDepositPayment(Guid addendumId)
        {
            ResultModel resultModel = new ResultModel();
            TblAddendum tblAddendum = await _addendumRepo.Get(addendumId);
            
            double amount = 0;
            if (tblAddendum == null)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum Id invalid.";
                return resultModel;
            }
            else
            {
                amount = (double)tblAddendum.Deposit;
            }
            if (tblAddendum.Status.Equals(Status.READY))
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum already paid deposit.";
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
            string redirectUrl = "https://ggarden.shop/thanks";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-deposit-payment-reponse";
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

        public async Task<ResultModel> CreateRentPayment(Guid addendumId, double? amount)
        {
            ResultModel resultModel = new ResultModel();
            TblAddendum tblAddendum = await _addendumRepo.Get(addendumId);
            if (tblAddendum.Status.Equals(Status.UNPAID))
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Please complete deposit first.";
                return resultModel;
            }
            if (tblAddendum.RemainMoney == 0)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum is fully paid.";
                return resultModel;
            }
            if (amount <= 1000 || amount == null)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Amount must be greater than 1000 for rent payment";
                return resultModel;
            }
            if (tblAddendum == null)
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
                PayAmount = (double)amount
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
            string redirectUrl = "https://ggarden.shop/thanks";
            string ipnUrl = "https://greengarden2023.azurewebsites.net/payment/receive-rent-payment-reponse";
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
            resultModel.Message = "Create rent payment success.";
            resultModel.Data = resJSON;
            return resultModel;
        }

        public async Task<ResultModel> CreateSalePayment(Guid addendumId)
        {
            ResultModel resultModel = new ResultModel();
            TblAddendum tblAddendum = await _addendumRepo.Get(addendumId);
            if (tblAddendum == null)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum Id invalid.";
                return resultModel;
            }
            if (tblAddendum.Status.Equals(Status.UNPAID))
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Please complete deposit first.";
                return resultModel;
            }
            if (tblAddendum.RemainMoney == 0)
            {
                resultModel.Code = 400;
                resultModel.IsSuccess = false;
                resultModel.Message = "Addendum is fully paid.";
                return resultModel;
            }
            double amount = (double)tblAddendum.RemainMoney;
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

        public async Task<bool> ProcessDepositPayment(MoMoResponseModel moMoResponseModel)
        {
            var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
            var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
            var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
            if (orderModel != null && moMoResponseModel.resultCode == 0)
            {
                var updateAddendum = await _addendumRepo.UpdateDepositAddendumPayment(orderModel.OrderId, orderModel.PayAmount);
                if (updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.OrderId,
                        Amount = orderModel.PayAmount,
                        Type = "MoMo deposit payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.MOMO

                    };
                    await _transactionRepo.Insert(tblTransaction);
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
            };
        }

        public async Task<ResultModel> ProcessDepositPaymentCash(Guid addendumId)
        {
            ResultModel result = new ResultModel();
            var orderModel = await _addendumRepo.Get(addendumId);
            if (orderModel != null )
            {
                if (!orderModel.Status.Equals(Status.UNPAID)) {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Addendum already paid deposit.";
                    return result;
                }
                var updateAddendum = await _addendumRepo.UpdateDepositAddendumPayment(orderModel.Id, (double)orderModel.Deposit);
                if (updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.Id,
                        Amount = orderModel.Deposit,
                        Type = "Cash deposit payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.CASH

                    };
                    await _transactionRepo.Insert(tblTransaction);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Deposit payment success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Deposit payment failed.";
                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Addendum not found.";
                return result;
            };
        }

        public async Task<bool> ProcessRentPayment(MoMoResponseModel moMoResponseModel)
        {
            var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
            var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
            var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
            if (orderModel != null && moMoResponseModel.resultCode == 0)
            {
                var updateAddendum = await _addendumRepo.UpdateRentAddendumPayment(orderModel.OrderId, orderModel.PayAmount);
                if(updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.OrderId,
                        Amount = orderModel.PayAmount,
                        Type = "Rent payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.MOMO

                    };
                    await _transactionRepo.Insert(tblTransaction);
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

        public async Task<ResultModel> ProcessRentPaymentCash(Guid addendumId, double? amount)
        {
            ResultModel result = new ResultModel();
            if (amount < 1000 ||  amount == null)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Amount must be greater than 1000.";
                return result;
            }
            var orderModel = await _addendumRepo.Get(addendumId);
            if (amount > orderModel.RemainMoney)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Amount exceed remain amount.";
                return result;
            }
            if (!orderModel.Status.Equals(Status.READY))
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Please complete deposit payment first.";
                return result;
            }
            if (orderModel != null)
            {
                var updateAddendum = await _addendumRepo.UpdateRentAddendumPayment(orderModel.Id, (double)amount);
                if (updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.Id,
                        Amount = amount,
                        Type = "Rent cash payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.MOMO

                    };
                    await _transactionRepo.Insert(tblTransaction);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Rent payment success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Rent payment failed.";
                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Addendum not found.";
                return result;
            }
        }

        public async Task<bool> ProcessSalePayment(MoMoResponseModel moMoResponseModel)
        {
            var base64OrderBytes = Convert.FromBase64String(moMoResponseModel.extraData ?? "");
            var orderJson = System.Text.Encoding.UTF8.GetString(base64OrderBytes);
            var orderModel = JsonConvert.DeserializeObject<MoMoOrderModel>(orderJson);
            if (orderModel != null && moMoResponseModel.resultCode == 0)
            {
                var updateAddendum = await _addendumRepo.UpdateSaleAddendumPayment(orderModel.OrderId, orderModel.PayAmount);
                if (updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.OrderId,
                        Amount = orderModel.PayAmount,
                        Type = "Rent payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.MOMO

                    };
                    await _transactionRepo.Insert(tblTransaction);
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

        public async Task<ResultModel> ProcessSalePaymentCash(Guid addendumId)
        {
            ResultModel result = new ResultModel();
            var orderModel = await _addendumRepo.Get(addendumId);
            
            if (orderModel != null)
            {
                if (!orderModel.Status.Equals(Status.READY))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Please complete deposit payment first.";
                    return result;
                }
                double amount = (double)orderModel.RemainMoney;
                var updateAddendum = await _addendumRepo.UpdateSaleAddendumPayment(orderModel.Id, amount);
                if (updateAddendum.IsSuccess == true)
                {
                    TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime hoChiMinhTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
                    TblTransaction tblTransaction = new TblTransaction
                    {
                        AddendumId = orderModel.Id,
                        Amount = amount,
                        Type = "Sale cash payment",
                        Status = TransactionType.RECEIVED,
                        DatetimePaid = hoChiMinhTime,
                        PaymentId = PaymentMethod.MOMO

                    };
                    await _transactionRepo.Insert(tblTransaction);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Sale payment success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Sale payment failed.";
                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Addendum not found.";
                return result;
            }
        }
    }
}
