using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.FeedbackRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.FeedbackService
{
    public class FeedbackService : IFeedbackService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IFeedbackRepo _fbRepo;
        private readonly IProductItemRepo _proItemRepo;
        private readonly IImageRepo _imgRepo;

        public FeedbackService(IFeedbackRepo fbRepo, IProductItemRepo proItemRepo, IImageRepo imgRepo)
        {
            _fbRepo = fbRepo;
            _proItemRepo = proItemRepo;
            _imgRepo = imgRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> changeStatus(string token, FeedbackChangeStatusModel model)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                result.IsSuccess = await _fbRepo.ChangeStatus(model);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> createFeedback(string token, FeedbackCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                    if (!userRole.Equals(Commons.CUSTOMER))
                    {
                        return new ResultModel()
                        {
                            IsSuccess = false,
                            Message = "User not allowed"
                        };
                    }
                }
                else
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                var productItem = await _proItemRepo.Get(model.ProductItemID);
                string userID = _decodeToken.Decode(token, "userid");
                if (productItem == null)
                {
                    result.IsSuccess = true;
                    result.Message = "Sản phẩm không tồn tại!";
                    return result;
                }
                var newFeedback = new TblFeedBack()
                {
                    Id = Guid.NewGuid(),
                    Comment = model.Comment,
                    CreateDate = DateTime.Now,
                    ProductItemId = model.ProductItemID,
                    Rating = model.Rating,
                    Status = Status.ACTIVE,
                    UserId = Guid.Parse(userID),
                };
                await _fbRepo.Insert(newFeedback);

                foreach (var url in model.ImagesUrls)
                {
                    TblImage newImg = new()
                    {
                        ImageUrl = url,
                        FeedbackId = newFeedback.Id,
                    };
                    await _imgRepo.Insert(newImg);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = newFeedback;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getListFeedback(string token, PaginationRequestModel pagingModel, Guid productItemID)
        {
            var result = new ResultModel();
            try
            {
                var listFb = await _fbRepo.GetFeedBacks(pagingModel, productItemID);
                var res = new List<ProItemFeedbackResModel>();

                foreach (var i in listFb.Results)
                {
                    var imageURL = await _imgRepo.GetImgUrlFeedback(i.Id);
                    var fbRecord = new ProItemFeedbackResModel()
                    {
                        ID = i.Id,
                        Rating = i.Rating,
                        Comment = i.Comment,
                        CreateDate = i.CreateDate,
                        Status = i.Status,
                        ImageURL = imageURL,
                        ProductItemID = i.ProductItemId
                    };
                    res.Add(fbRecord);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = res;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
