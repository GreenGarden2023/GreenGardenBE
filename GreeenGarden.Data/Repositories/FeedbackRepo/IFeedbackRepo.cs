using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.FeedbackRepo
{
    public interface IFeedbackRepo : IRepository<TblFeedBack>
    {
        Task<bool> ChangeStatus(FeedbackChangeStatusModel model);
        Task<List<TblFeedBack>> GetFeedBackByProductItemDetail(Guid productItemDetailID);
    }
}
