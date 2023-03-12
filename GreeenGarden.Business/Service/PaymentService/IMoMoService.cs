using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        Task<ResultModel> DepositPaymentMoMo(MoMoDepositModel moMoDepositModel);

        Task<ResultModel> DepositPaymentCash(MoMoDepositModel moMoDepositModel);

        Task<ResultModel> OrderPaymentCash(MoMoPaymentModel moMoPaymentModel);

    }
}
