using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        Task<ResultModel> CreateDepositPaymentMoMo(Guid orderID);
    }
}
