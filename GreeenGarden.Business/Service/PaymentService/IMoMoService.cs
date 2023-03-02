
using GreeenGarden.Data.Models.MoMoModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        public Task<string> CreateOrderPayment(Guid orderID);
        public Task<bool> ProcessOrderPayment(MoMoResponseModel moMoResponseModel);
    }
}
