﻿using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.FeedbackService
{
    public interface IFeedbackService
    {
        public Task<ResultModel> createFeedback(string token, FeedbackCreateModel model);
        public Task<ResultModel> getListFeedbackByProductItem(string token, PaginationRequestModel pagingModel, Guid productItemID);
        public Task<ResultModel> getListFeedbackByManager(string token, PaginationRequestModel pagingModel);
        public Task<ResultModel> getListFeedbackByOrder(string token, PaginationRequestModel pagingModel, Guid orderID);
        public Task<ResultModel> changeStatus(string token, FeedbackChangeStatusModel model);
    }
}
