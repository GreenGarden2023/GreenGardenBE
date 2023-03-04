
using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        public Task<ResultModel> CreateOrderPayment(Guid orderID);
        public Task<bool> ProcessOrderPayment(MoMoResponseModel moMoResponseModel);
        public Task<ResultModel> CreateAddendumPayment(Guid addendumId, double amount);
        public Task<bool> ProcessAddendumPayment(MoMoResponseModel moMoResponseModel);
    }
}
