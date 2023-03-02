using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.EMailService
{
    public interface IEMailService
    {
        public Task<ResultModel> SendEmailVerificationOTP(string email);
        public Task<ResultModel> VerifyEmailVerificationOTP(string code);
    }
}

