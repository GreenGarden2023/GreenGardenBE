
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        public Task<ResultModel> CreateRentPayment(Guid addendumId, double amount);
        public Task<bool> ProcessRentPayment(MoMoResponseModel moMoResponseModel);
        public Task<ResultModel> CreateRentDepositPayment(Guid addendumId);
        public Task<bool> ProcessRentDepositPayment(MoMoResponseModel moMoResponseModel);
    }
}
