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

        Task<ResultModel> WholeOrderPaymentMoMo(MoMoPaymentModel moMoPaymentModel);

        Task<ResultModel> DepositPaymentCash(MoMoDepositModel moMoDepositModel);

        Task<ResultModel> OrderPaymentCash(MoMoPaymentModel moMoPaymentModel);

        Task<ResultModel> WholeOrderPaymentCash(MoMoPaymentModel moMoPaymentModel);

        //--Takecare combo order--//
        Task<ResultModel> TakecareComboOrderDepositPaymentCash(Guid orderID);
        Task<ResultModel> TakecareComboOrderDepositPaymentMoMo(Guid orderID);

        Task<ResultModel> TakecareComboOrderPaymentCash(Guid orderID, double amount);
        Task<ResultModel> TakecareComboOrderPaymentMoMo(Guid orderID, double amount);

        Task<ResultModel> TakecareComboOrderWholePaymentCash(Guid orderID);
        Task<ResultModel> TakecareComboOrderWholePaymentMoMo(Guid orderID);

        Task<bool> ProcessTakecareComboOrderDepositPaymentMoMo(MoMoResponseModel moMoResponseModel);
        Task<bool> ProcessTakecareComboOrderPaymentMoMo(MoMoResponseModel moMoResponseModel);
    }
}
