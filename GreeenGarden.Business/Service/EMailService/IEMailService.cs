using GreeenGarden.Data.Models.FileModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.EMailService
{
    public interface IEMailService
    {
        Task<ResultModel> SendEmailVerificationOTP(string email);
        Task<ResultModel> SendEmailRegisterVerificationOTP(string email, string userName);
        Task<ResultModel> SendEmailServiceUpdate(string email, string serviceCode);
        Task<ResultModel> SendEmailReportUpdate(string email, Guid serviceCalendarId);
        Task<ResultModel> VerifyEmailVerificationOTP(string email, string code);
        Task<ResultModel> SendEmailRentOrderContract(string email, Guid orderID, FileData file);
        Task<ResultModel> SendEmailSupport(string supportName, string supportPhone);
    }
}

