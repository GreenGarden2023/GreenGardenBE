
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        public Task<ResultModel> CreateRentPayment(Guid addendumId, double? amount);
        public Task<ResultModel> CreateSalePayment(Guid addendumId);
        public Task<bool> ProcessRentPayment(MoMoResponseModel moMoResponseModel);
        public Task<bool> ProcessSalePayment(MoMoResponseModel moMoResponseModel);
        public Task<ResultModel> CreateDepositPayment(Guid addendumId);
        public Task<bool> ProcessDepositPayment(MoMoResponseModel moMoResponseModel);
    }
}
