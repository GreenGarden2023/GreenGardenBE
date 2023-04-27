
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.EmailOTPCodeRepo;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
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
        public EMailService(IServiceRepo serviceRepo, IEmailOTPCodeRepo emailOTPCodeRepo, IUserRepo userRepo, IServiceCalendarRepo serviceCalendarRepo, IServiceOrderRepo serviceOrderRepo)
        {
            _emailOTPCodeRepo = emailOTPCodeRepo;
            _userRepo = userRepo;
            _serviceRepo = serviceRepo;
            _serviceCalendarRepo = serviceCalendarRepo;
            _serviceOrderRepo = serviceOrderRepo;
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
    }
}

