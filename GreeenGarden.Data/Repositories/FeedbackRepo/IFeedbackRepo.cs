using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using EntityFrameworkPaginateCore;

namespace GreeenGarden.Data.Repositories.FeedbackRepo
{
    public interface IFeedbackRepo : IRepository<TblFeedBack>
    {
        Task<bool> ChangeStatus(FeedbackChangeStatusModel model);
        Task<List<TblFeedBack>> GetFeedBackByProductItemDetail(Guid productItemDetailID);
        Task<Page<TblFeedBack>> GetFeedBackByOrderID(Guid orderID, PaginationRequestModel pagingModel);
        Task<List<FeedbackOrderResModel>> GetFeedBackOrderDetail(Guid orderId, Guid productItemDetailID);
        Task<bool> UpdateFeedback(TblFeedBack entity);
    }
}
