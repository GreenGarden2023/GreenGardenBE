using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.FeedbackService
{
    public interface IFeedbackService
    {
        public Task<ResultModel> createFeedback(string token, FeedbackCreateModel model);
        public Task<ResultModel> getListFeedback(string token, PaginationRequestModel pagingModel, Guid productItemID);
        public Task<ResultModel> changeStatus(string token, FeedbackChangeStatusModel model);
    }
}
