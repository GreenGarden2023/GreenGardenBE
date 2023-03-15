using GreeenGarden.Data.Models.MoMoModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.PaymentService
{
    public interface IMoMoService
    {
        Task<ResultModel> DepositPaymentMoMo(MoMoDepositModel moMoDepositModel);
        Task<bool> ProcessDepositPaymentMoMo(MoMoResponseModel moMoResponseModel);

        Task<ResultModel> OrderPaymentMoMo(MoMoPaymentModel moMoPaymentModel);
        Task<bool> ProcessOrderPaymentMoMo(MoMoResponseModel moMoResponseModel);

        Task<ResultModel> WholeOrderPaymentMoMo(MoMoWholeOrderModel moMoWholeOrderModel);

        Task<ResultModel> DepositPaymentCash(MoMoDepositModel moMoDepositModel);

        Task<ResultModel> OrderPaymentCash(MoMoPaymentModel moMoPaymentModel);

        Task<ResultModel> WholeOrderPaymentCash(MoMoWholeOrderModel moMoWholeOrderModel);
    }
}
