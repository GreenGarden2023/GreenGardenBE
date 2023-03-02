using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.EmailOTPCodeRepo;
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
        public EMailService(IEmailOTPCodeRepo emailOTPCodeRepo, IUserRepo userRepo)
        {
            _emailOTPCodeRepo = emailOTPCodeRepo;
            _userRepo = userRepo;
        }
        public async Task<ResultModel> SendEmailVerificationOTP(string email)
        {
            ResultModel result = new ResultModel();
            Random rnd = new();

            string OTP = (rnd.Next(000000, 999999)).ToString();

            var checkEmail = await _userRepo.CheckUserEmail(email);
            if (checkEmail == false)
            {
                result.IsSuccess = false;
                result.Code = 404;
                result.Message = "Email Invalid.";
                return result;
            }

            string from = "ggarden.shop2023@gmail.com";
            string password = "wwgnydlcgvvdfhyq";

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
                "<h3>You requested a password reset. </h3>" +
                "<p>Please use the code below to reset your password.</p>" +
                "<p>Your code is: " + OTP + "</p>" +
                "</body>" +
                "</html>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            var emailCode = new TblEmailOtpcode()
            {
                Email = email,
                Optcode = OTP,
            };
            await _emailOTPCodeRepo.Insert(emailCode);
            result.IsSuccess = true;
            result.Code = 200;
            result.Message = OTP;

            return result;
        }

        public async Task<ResultModel> VerifyEmailVerificationOTP(string code)
        {
            ResultModel result = new ResultModel();
            var verify = await _emailOTPCodeRepo.DeleteCode(code);
            if (!String.IsNullOrEmpty(verify))
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
    }
}

