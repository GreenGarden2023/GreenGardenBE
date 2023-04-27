
using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FileModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.EmailOTPCodeRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GreeenGarden.Business.Service.EMailService
{
    public class EMailService : IEMailService
    {
        private readonly IEmailOTPCodeRepo _emailOTPCodeRepo;
        private readonly IUserRepo _userRepo;
        private readonly IServiceRepo _serviceRepo;
        private readonly IServiceCalendarRepo _serviceCalendarRepo;
        private readonly IServiceOrderRepo _serviceOrderRepo;
        private readonly IRentOrderRepo _rentOrderRepo;
        private readonly IRentOrderDetailRepo _rentOrderDetailRepo;
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        private readonly IProductItemRepo _productItemRepo;
        private readonly ISizeRepo _sizeRepo;
        public EMailService(ISizeRepo sizeRepo, IProductItemRepo productItemRepo, IProductItemDetailRepo productItemDetailRepo,  IRentOrderRepo rentOrderRepo, IRentOrderDetailRepo rentOrderDetailRepo, IServiceRepo serviceRepo, IEmailOTPCodeRepo emailOTPCodeRepo, IUserRepo userRepo, IServiceCalendarRepo serviceCalendarRepo, IServiceOrderRepo serviceOrderRepo)
        {
            _emailOTPCodeRepo = emailOTPCodeRepo;
            _userRepo = userRepo;
            _serviceRepo = serviceRepo;
            _serviceCalendarRepo = serviceCalendarRepo;
            _serviceOrderRepo = serviceOrderRepo;
            _rentOrderRepo = rentOrderRepo;
            _rentOrderDetailRepo = rentOrderDetailRepo;
            _productItemDetailRepo = productItemDetailRepo;
            _productItemRepo = productItemRepo;
            _sizeRepo = sizeRepo;
        }

        public async Task<ResultModel> SendEmailRegisterVerificationOTP(string email, string userName)
        {
            ResultModel result = new();
            Random rnd = new();

            string OTP = rnd.Next(000000, 999999).ToString();

            bool checkEmail = await _userRepo.CheckUserEmail(email);
            if (checkEmail == false)
            {
                result.IsSuccess = false;
                result.Code = 404;
                result.Message = "Email Invalid.";
                return result;
            }

            string from = SecretService.SecretService.GetEmailCred().EmailAddress;
            string password = SecretService.SecretService.GetEmailCred().EmailPassword;

            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(from));
            message.Subject = "GreenGarden Yêu cầu chăm sócác cập nhật yêu cầu chăm sóc đăng ký.";
            message.To.Add(MailboxAddress.Parse(email));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text =
                "<html>" +
                "<body>" +
                "<h1>GreenGarden<h1>" +
                "<h3>Bạn vừa tạo tài khoản trên GreenGarden: " + userName + ".</h3>" +
                "<p>Vui lòng sử dụng mã bên dưới để xác minh tài khoản của bạn.</p>" +
                "<p>Mã của bạn là: " + OTP + "</p>" +
                "</body>" +
                "</html>"
            };

            using SmtpClient smtp = new();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            _ = await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            TblEmailOtpcode emailCode = new()
            {
                Email = email,
                Optcode = OTP,
            };
            _ = await _emailOTPCodeRepo.Insert(emailCode);
            result.IsSuccess = true;
            result.Code = 200;
            result.Message = OTP;

            return result;
        }


        public async Task<ResultModel> SendEmailServiceUpdate(string email, string serviceCode)
        {
            ResultModel result = new();
            try
            {
                TblService tblService = await _serviceRepo.GetServiceByServiceCode(serviceCode);
                string from = SecretService.SecretService.GetEmailCred().EmailAddress;
                string password = SecretService.SecretService.GetEmailCred().EmailPassword;
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "GreenGarden cập nhật yêu cầu chăm sóc.";
                message.To.Add(MailboxAddress.Parse(email));
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text =
                    "<html>" +
                    "<body>" +
                    "<h1>GreenGarden<h1>" +
                    "<h3>Yêu cầu chăm sóc " + serviceCode + " đã được cập nhật.</h3>" +
                    "<p>Vui lòng xác nhận qua đường dẫn bên dưới: </p>" +
                    "<p> https://ggarden.shop/take-care-service/me/" + tblService.Id + " </p>" +
                    "<p> Best regards,</p>" +
                    "<h3>GreenGarden.</h3>" +
                    "</body>" +
                    "</html>"
                };

                using SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, password);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Email send successful";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = e.ToString();
                return result;
            }
        }

        public async Task<ResultModel> SendEmailVerificationOTP(string email)
        {
            ResultModel result = new();
            Random rnd = new();

            string OTP = rnd.Next(000000, 999999).ToString();

            bool checkEmail = await _userRepo.CheckUserEmail(email);
            if (checkEmail == false)
            {
                result.IsSuccess = false;
                result.Code = 404;
                result.Message = "Email Invalid.";
                return result;
            }

            string from = SecretService.SecretService.GetEmailCred().EmailAddress;
            string password = SecretService.SecretService.GetEmailCred().EmailPassword;

            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(from));
            message.Subject = "GreenGarden account email verification code";
            message.To.Add(MailboxAddress.Parse(email));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text =
                "<html>" +
                "<body>" +
                "<h1>GreenGarden<h1>" +
                "<h3>Bạn dã yêu cầu đặt lại mật khẩu. </h3>" +
                "<p>Vui lòng sử dụng mã bên dưới để xác nhận.</p>" +
                "<p>Mã của bạn là: " + OTP + "</p>" +
                "</body>" +
                "</html>"
            };

            using SmtpClient smtp = new();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            _ = await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            TblEmailOtpcode emailCode = new()
            {
                Email = email,
                Optcode = OTP,
            };
            _ = await _emailOTPCodeRepo.Insert(emailCode);
            result.IsSuccess = true;
            result.Code = 200;
            result.Message = OTP;
            return result;
        }

        public async Task<ResultModel> VerifyEmailVerificationOTP(string email, string code)
        {
            ResultModel result = new();
            string verify = await _emailOTPCodeRepo.DeleteCode(email, code);
            if (!string.IsNullOrEmpty(verify))
            {
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = verify;
                result.Message = "Code verified successfully.";
                return result;
            }
            else
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "Code verified failed.";
                return result;
            }
        }

        public async Task<ResultModel> SendEmailReportUpdate(string email, Guid serviceCalendarId)
        {
            ResultModel result = new();
            try
            {
                TblServiceCalendar tblServiceCalendar = await _serviceCalendarRepo.Get(serviceCalendarId);
                TblServiceOrder tblServiceOrder = await _serviceOrderRepo.Get(tblServiceCalendar.ServiceOrderId);
                string from = SecretService.SecretService.GetEmailCred().EmailAddress;
                string password = SecretService.SecretService.GetEmailCred().EmailPassword;
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "GreenGarden cập nhật chăm sóc";
                message.To.Add(MailboxAddress.Parse(email));
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text =
                    "<html>" +
                    "<body>" +
                    "<h1>GreenGarden<h1>" +
                    "<h3>Lịch chăm sóc ngày " + tblServiceCalendar.ServiceDate + " cho đơn hàng " + tblServiceOrder.OrderCode + " đã có cập nhật.</h3>" +
                    "<p>Vui lòng kiểm tra cập nhật tại: </p>" +
                    "<p>https://ggarden.shop/order/service/"+ tblServiceOrder.Id + "</p>" +
                    "<p> Trân trọng,</p>" +
                    "<h3>GreenGarden.</h3>" +
                    "</body>" +
                    "</html>"
                };

                using SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, password);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Email send successful";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> SendEmailRentOrderContract(string email, Guid orderID, FileData file)
        {
            ResultModel result = new();
            try
            {
                TblRentOrder tblRentOrder = await _rentOrderRepo.Get(orderID);
                if (tblRentOrder == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "OrderID invalid.";
                    return result;
                }
                TblUser tblUser = await _userRepo.Get((Guid)tblRentOrder.UserId);
                List<TblRentOrderDetail> tblRentOrderDetails = await _rentOrderDetailRepo.GetRentOrderDetailsByRentOrderID(tblRentOrder.Id); ;


                string from = SecretService.SecretService.GetEmailCred().EmailAddress;
                string password = SecretService.SecretService.GetEmailCred().EmailPassword;
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "GreenGarden hợp đồng thuê cây";
                message.To.Add(MailboxAddress.Parse(email));



                string htmlContent = "";
                htmlContent += "<html>";
                htmlContent += "<body>";

                htmlContent += "<div style='width:100%'>";
                htmlContent += "<h1>HỢP ĐỒNG THUÊ CÂY</h1>";
                htmlContent += "<p style='width:100%;'>Cảm ơn bạn đã sử dụng dịch vụ thuê cây của chúng tôi. Đơn hàng "+ tblRentOrder.OrderCode + " của bạn được tạo lúc "+tblRentOrder.CreateDate.Value.ToShortDateString()+".</p>";
                htmlContent += "<p>Vui lòng xem hợp đồng thuê bên dưới hoặc qua file đính kèm <br></p";
                htmlContent += "</div>";

                htmlContent += "<div style='border:2px solid #000; padding: 20px'>";
                htmlContent += "<h2 style='width:100%;text-align:center'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</h2>";
                htmlContent += "<h3 style='width:100%; text-align:center'>Độc lập – Tự do – Hạnh phúc</h3>";
                htmlContent += "<p style='width:100%; text-align:right'>Hồ Chí Minh, " + tblRentOrder.CreateDate.Value.ToShortDateString() + "</p>";
                htmlContent += "<h2 style='width:100%;text-align:center'>HỢP ĐỒNG THUÊ CÂY </h2>";

                htmlContent += "<p style='width:100%;'>Các bên tham gia hợp đồng gồm:</p>";

                htmlContent += "<table style='width:100%'>";
                htmlContent += "<tr>";

                htmlContent += "<td style='vertical-align: top;'>";
                htmlContent += "<h3 style='width:100%;'>BÊN CHO THUÊ (Bên A)</h3>";
                htmlContent += "<p>Tên: Green Garden</p>";
                htmlContent += "<p>Địa chỉ: Đại Học FPT</p>";
                htmlContent += "<p>Điện thoại: 0909000999</p>";
                htmlContent += "</td>";

                htmlContent += "<td style=' vertical-align: top;'>";
                htmlContent += "<h3 style='width:100%;'>BÊN THUÊ (Bên B) </h3>";
                htmlContent += "<p>Tên khách hàng: " + tblUser.FullName + " </p>";
                htmlContent += "<p>Địa chỉ: " + tblUser.Address + " </p>";
                htmlContent += "<p>Điện thoại:" + tblUser.Phone + " <br></p>";
                htmlContent += "</td>";

                htmlContent += "</tr>";
                htmlContent += "</table>";

                htmlContent += "<p>Hai bên thống nhất thỏa thuận nội dung hợp đồng như sau:</p>";
                htmlContent += "<h3>Điều 1: Điều khoản chung</h3>";
                htmlContent += "<p>- Nếu trong quá trình thuê cây có vấn đề như hư, héo, chết, .... thì bên B sẽ chịu trách nhiệm hoàn toàn tuỳ thuộc vào tình trạng của cây.<br>" +
                    "- Bên B phải kiểm tra kĩ cây trước khi nhận. Nếu có vấn đề thì phải báo cho bên A, cây sẽ được đổi cây mới không phụ thu bất kì chi phí nào.<br>" +
                    "- Nếu bên B không kiểm tra kĩ cây trước khi nhận thì khi cây có vấn đề thì bên B phải chịu trách nhiệm.<br>" +
                    "- Nếu cây không được trả đúng hạn thì sẽ phụ thu thêm tiền cho các ngày tiếp theo đến khi nào cây được trả.<br>" +
                    "- Bên B có thể tự mình gia hạn thêm thời gian thuê trên hệ thống.<br>" +
                    "- Khi gia hạn thuê thì chỉ được chọn những cây đang thuê, không được thêm bất cứ cây nào khác nếu thêm thì sẽ tạo đơn hàng mới.<br>" +
                    "- Khi thuê cây bên B phải cọc 20% giá trị đơn hàng. Khi trả cây thì bên A sẽ trả lại cọc cho bên B.<br>" +
                    "- Bên B đặt đơn xong vui lòng thanh toán cọc. Đơn hàng chỉ được giao khi bên B đã thanh toán cọc.<br>" +
                    "- Nếu trong quá trình thuê cây có vấn đề thì phải báo gấp cho bên A biết để kịp thời cứu chữa.<br></p>";
                int count = 2;
                foreach (TblRentOrderDetail tblRentOrderDetail in tblRentOrderDetails)
                {
                    TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get((Guid)tblRentOrderDetail.ProductItemDetailId);
                    TblProductItem tblProductItem = await _productItemRepo.Get(tblProductItemDetail.ProductItemId);
                    if (!String.IsNullOrEmpty(tblProductItem.Rule))
                    {
                        htmlContent += "<h3>Điều " + count + ": Đối với cây " + tblProductItem.Name + "</h3>";


                        string a = tblProductItem.Rule;
                        List<string> splitted = a.Split('.').ToList();

                        foreach (string b in splitted)
                        {
                            htmlContent += "-<p>" + b + ".</p>";
                        }
                        count++;
                    }
                }

                htmlContent += "<h3>Những điều khoản trên được áp dụng với những sản phẩm:</h3>";


                htmlContent += "<table style ='width:100%; border: 1px solid #000; border-collapse: collapse'>";
                htmlContent += "<thead style='font-weight:bold; border-collapse: collapse'>";
                htmlContent += "<tr>";
                htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'> Tên sản phẩm </td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'> Kích thước </td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Giá thuê 1 ngày</td>";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Số lượng</td >";
                htmlContent += "<td style='border:1px solid #000;border-collapse: collapse; text-align:center'>Tổng tiền 1 ngày</td>";
                htmlContent += "</tr>";
                htmlContent += "</thead >";
                htmlContent += "<tbody>";

                foreach (TblRentOrderDetail tblRentOrderDetail in tblRentOrderDetails)
                {
                    TblProductItemDetail tblProductItemDetail = await _productItemDetailRepo.Get((Guid)tblRentOrderDetail.ProductItemDetailId);
                    TblSize tblSize = await _sizeRepo.Get(tblProductItemDetail.SizeId);
                    TblProductItem tblProductItem = await _productItemRepo.Get(tblProductItemDetail.ProductItemId);
                    htmlContent += "<tr>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblProductItem.Name + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblSize.Name + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.RentPricePerUnit + "</td >";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.Quantity + "</td>";
                    htmlContent += "<td style='border:1px solid #000; border-collapse: collapse; text-align:center'>" + tblRentOrderDetail.TotalPrice + "</td >";
                    htmlContent += "</tr>";
                }
                htmlContent += "</tbody>";
                htmlContent += "</table>";

                htmlContent += "<p style='text-align:right;'>Thuê từ " + tblRentOrder.StartDateRent.ToShortDateString() + " đến: " + tblRentOrder.EndDateRent.ToShortDateString() + "</p>";
                htmlContent += "<p style='text-align:right;'>Số tiền được giảm: " + tblRentOrder.DiscountAmount + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Phí vận chuyển: " + tblRentOrder.TransportFee + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Tổng cộng: " + tblRentOrder.TotalPrice + "đ</p>";
                htmlContent += "<p style='text-align:right;'>Tiền cọc: " + tblRentOrder.Deposit + "đ</p>";


                htmlContent += "<table style='width:100%'>";
                htmlContent += "<tr>";

                htmlContent += "<td style='text-align:center;'>";
                htmlContent += "<h3 style='width:100%;'>ĐẠI DIỆN BÊN A</h3>";
                htmlContent += "<p> Green Garden  </p>";
                htmlContent += "</td>";

                htmlContent += "<td style=' text-align:center;'>";
                htmlContent += "<h3 style='width:100%;'> ĐẠI DIỆN BÊN B </h3>";
                htmlContent += "<p>" + tblUser.FullName + "</p>";
                htmlContent += "</td>";

                htmlContent += "</tr>";
                htmlContent += "</table>";

                htmlContent += "</div>";
                htmlContent += "</body>";
                htmlContent += "</html>";
                

                var pdfAttachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(new MemoryStream(file.bytes)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName("HOP_DONG_THUE_CAY.pdf")
                };


                var multipart = new Multipart("mixed");
                multipart.Add(new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlContent });
                multipart.Add(pdfAttachment);
                message.Body = multipart;


                using SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, password);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Email send successful";
                return result;
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

