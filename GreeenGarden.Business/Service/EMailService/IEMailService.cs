using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.EMailService
{
    public interface IEMailService
    {
         Task<ResultModel> SendEmailVerificationOTP(string email);
        Task<ResultModel> SendEmailServiceUpdate(string email, string serviceCode);
        Task<ResultModel> VerifyEmailVerificationOTP(string code);
    }
}

