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
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
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
        private readonly IProductItemDetailRepo _proItemDetailRepo;
        private readonly IImageRepo _imgRepo;
        private readonly ISaleOrderRepo _saleOrdRepo;
        private readonly ISaleOrderDetailRepo _saleOrdDetailRepo;
        private readonly IRentOrderRepo _rentOrdRepo;
        private readonly IRentOrderDetailRepo _rentOrdDetailRepo;

        public FeedbackService(IFeedbackRepo fbRepo, IProductItemRepo proItemRepo, IImageRepo imgRepo, ISaleOrderRepo saleOrdRepo,
                               ISaleOrderDetailRepo saleOrdDetailRepo, IRentOrderRepo rentOrdRepo, IRentOrderDetailRepo rentOrdDetailRepo
                            , IProductItemDetailRepo proItemDetailRepo)
        {
            _fbRepo = fbRepo;
            _proItemRepo = proItemRepo;
            _proItemDetailRepo = proItemDetailRepo;
            _imgRepo = imgRepo;
            _decodeToken = new DecodeToken();
            _saleOrdRepo = saleOrdRepo;
            _saleOrdDetailRepo = saleOrdDetailRepo;
            _rentOrdRepo = rentOrdRepo;
            _rentOrdDetailRepo = rentOrdDetailRepo;
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
                bool flag = false;
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

                var productItemDetail = await _proItemDetailRepo.Get(model.ProductItemDetailID);
                string userID = _decodeToken.Decode(token, "userid");
                if (productItemDetail == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Sản phẩm không còn tồn tại!";
                    return result;
                }

                var order = new object();
                var saleOrder = await _saleOrdRepo.Get(model.OrderID);
                if (saleOrder != null)
                {
                    flag = true;
                    if (!saleOrder.Status.Equals(Status.COMPLETED))
                    {
                        result.IsSuccess = false;
                        result.Message = "Đơn hàng chưa hoàn tất";
                        return result;
                    }
                    var listSaleOrderDetail = await _saleOrdDetailRepo.GetSaleOrderDetails(saleOrder.Id);
                    var saleOrderDetail = new TblSaleOrderDetail();
                    if ((DateTime.Now - (DateTime) saleOrder.CreateDate).TotalDays >= 30)
                    {
                        result.IsSuccess = false;
                        result.Message = "Đã quá hạn để feedback";
                        return result;
                    }
                    foreach (var i in listSaleOrderDetail)
                    {
                        bool check = false;
                        if (model.ProductItemDetailID.Equals(i.ProductItemDetailID)) 
                        { 
                            check = true;
                            saleOrderDetail = await _saleOrdDetailRepo.Get(i.ID);
                        } // get saleOrderDetailID
                        
                        if (check == false)
                        {
                            result.IsSuccess = false;
                            result.Message = "Sản phẩm không có trong order";
                            return result;
                        }
                    }
                    if (saleOrderDetail.FeedbackStatus == true)
                    {
                        result.IsSuccess = false;
                        result.Message = "Cây này đã được đánh giá trước đó";
                        return result;
                    }
                    if (saleOrderDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Message = "không có đơn hàng chi tiết nào phù hợp";
                        return result;
                    }
                    saleOrderDetail.FeedbackStatus = true;
                    await _saleOrdDetailRepo.UpdateSaleOrderDetails(saleOrderDetail);
                    
                };
                var rentOrder = await _rentOrdRepo.Get(model.OrderID);
                if (rentOrder != null)
                {
                    flag = true;
                    if (!rentOrder.Status.Equals(Status.COMPLETED))
                    {
                        result.IsSuccess = false;
                        result.Message = "Đơn hàng chưa hoàn tất";
                        return result;
                    }
                    var listRentOrderDetail = await _rentOrdDetailRepo.GetRentOrderDetails(rentOrder.Id);
                    var rentOrderDetail = new TblRentOrderDetail();
                    if ((DateTime.Now - (DateTime)rentOrder.CreateDate).TotalDays >= 30)
                    {
                        result.IsSuccess = false;
                        result.Message = "Đã quá hạn để feedback";
                        return result;
                    }
                    foreach (var i in listRentOrderDetail)
                    {
                        bool check = false;
                        if (model.ProductItemDetailID.Equals(i.ProductItemDetailID))
                        {
                            check = true;
                            rentOrderDetail = await _rentOrdDetailRepo.Get(i.ID);
                        } // get saleOrderDetailID

                        if (check == false)
                        {
                            result.IsSuccess = false;
                            result.Message = "Sản phẩm không có trong order";
                            return result;
                        }
                    }
                    if (rentOrderDetail.FeedbackStatus == true)
                    {
                        result.IsSuccess = false;
                        result.Message = "Cây này đã được đánh giá trước đó";
                        return result;
                    }
                    if (rentOrderDetail == null)
                    {
                        result.IsSuccess = false;
                        result.Message = "không có đơn hàng chi tiết nào phù hợp";
                        return result;
                    }
                    rentOrderDetail.FeedbackStatus = true;
                    await _rentOrdDetailRepo.UpdateRentOrderDetail(rentOrderDetail);
                }

                if (flag == false)
                {
                    result.IsSuccess = false;
                    result.Message = "không tìm thấy đơn hàng nào";
                    return result;
                }


                var newFeedback = new TblFeedBack()
                {
                    Id = Guid.NewGuid(),
                    Comment = model.Comment,
                    CreateDate = DateTime.Now,
                    ProductItemDetailId = model.ProductItemDetailID,

                    Rating = model.Rating,
                    Status = Status.ACTIVE,
                    UserId = Guid.Parse(userID),
                    UpdateDate = null
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
                newFeedback.ProductItemDetail.TblRentOrderDetails.Clear();
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

        public Task<ResultModel> getListFeedbackByManager(string token, PaginationRequestModel pagingModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> getListFeedbackByOrder(string token, PaginationRequestModel pagingModel, Guid orderID)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultModel> getListFeedbackByProductItem(string token, PaginationRequestModel pagingModel, Guid productItemID)
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

                        //ProductItemID = i.ProductItemId
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
