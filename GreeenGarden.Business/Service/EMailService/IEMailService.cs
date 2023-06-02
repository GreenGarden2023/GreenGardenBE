using GreeenGarden.Data.Models.FileModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Business.Service.EMailService
{
    public interface IEMailService
    {
        Task<ResultModel> SendEmailVerificationOTP(string email);
        Task<ResultModel> SendEmailRegisterVerificationOTP(string email, string userName);
        Task<ResultModel> SendEmailServiceUpdate(string email, string serviceCode);
        Task<ResultModel> SendEmailComboServiceUpdate(string email, string serviceCode);
        Task<ResultModel> SendEmailReportUpdate(string email, Guid serviceCalendarId);
        Task<ResultModel> VerifyEmailVerificationOTP(string email, string code);
        Task<ResultModel> SendEmailRentOrderContract(string email, Guid orderID, FileData file);
        Task<ResultModel> SendEmailCareGuide(string email, Guid orderID, FileData file, int flag);
        Task<ResultModel> SendEmailServiceCareGuide(string email, Guid orderID, FileData file);
        Task<ResultModel> SendEmailComboServiceCareGuide(string email, Guid orderID, FileData file);
        Task<ResultModel> SendEmailCareGuideForService(string email, List<ServiceDetailResModel> listItem, FileData file);
        Task<ResultModel> SendEmailAssignTechnician(string email, string serviceCode);
        Task<ResultModel> SendEmailSupport(string supportName, string supportPhone);
    }
}

