using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Business.Service.TakecareService
{
    public interface ITakecareService
    {
        Task<ResultModel> CreateRequest(string token, ServiceInsertModel serviceInsertModel);
        Task<ResultModel> UpdateRequestStatus(string token, ServiceStatusModel serviceStatusModel);
        Task<ResultModel> AssignTechnician(string token, ServiceAssignModelManager serviceAssignModelManager);
        Task<ResultModel> UpdateServicePrice(string token, ServiceUpdateModelManager? serviceUpdateModelManager, List<ServiceDetailUpdateModelManager>? serviceDetailUpdateModelManagers);
        Task<ResultModel> GetRequestOrderByTechnician(string token, PaginationRequestModel pagingModel, Guid technicianID);
        Task<ResultModel> GetRequestOrderByServiceCode(string token, PaginationRequestModel pagingModel, ServiceSearchByCodeModel model);
        Task<ResultModel> GetUserRequest(string token);
        Task<ResultModel> GetAllRequest(string token);
        Task<ResultModel> CancelRequest(string token, CancelRequestModel model);
        Task<ResultModel> GetARequestDetail(string token, Guid serviceID);
        Task<ResultModel> GetRequestDetailByServiceOrder(string token, string serviceOrder);
    }
}

