using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.FeedbackRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using System.Security.Claims;

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
        private readonly IUserRepo _userRepo;
        private readonly ISizeRepo _sizeRepo;

        public FeedbackService(IFeedbackRepo fbRepo, IProductItemRepo proItemRepo, IImageRepo imgRepo, ISaleOrderRepo saleOrdRepo,
                               ISaleOrderDetailRepo saleOrdDetailRepo, IRentOrderRepo rentOrdRepo, IRentOrderDetailRepo rentOrdDetailRepo
                            , IProductItemDetailRepo proItemDetailRepo, IUserRepo userRepo, ISizeRepo sizeRepo)
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
            _userRepo = userRepo;
            _sizeRepo = sizeRepo;
        }

        public async Task<ResultModel> changeStatus(string token, FeedbackChangeStatusModel model)
        {
            ResultModel result = new();
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
            ResultModel result = new();
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

                TblProductItemDetail? productItemDetail = await _proItemDetailRepo.Get(model.ProductItemDetailID);
                string userID = _decodeToken.Decode(token, "userid");
                if (productItemDetail == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Sản phẩm không còn tồn tại!";
                    return result;
                }

                object order = new();
                TblSaleOrder? saleOrder = await _saleOrdRepo.Get(model.OrderID);
                if (saleOrder != null)
                {
                    flag = true;
                    if (!saleOrder.Status.Equals(Status.COMPLETED))
                    {
                        result.IsSuccess = false;
                        result.Message = "Đơn hàng chưa hoàn tất";
                        return result;
                    }
                    List<Data.Models.OrderModel.SaleOrderDetailResModel> listSaleOrderDetail = await _saleOrdDetailRepo.GetSaleOrderDetails(saleOrder.Id);
                    TblSaleOrderDetail? saleOrderDetail = new();
                    if ((DateTime.Now - (DateTime)saleOrder.CreateDate).TotalDays >= 30)
                    {
                        result.IsSuccess = false;
                        result.Message = "Đã quá hạn để feedback";
                        return result;
                    }
                    foreach (Data.Models.OrderModel.SaleOrderDetailResModel i in listSaleOrderDetail)
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
                    _ = await _saleOrdDetailRepo.UpdateSaleOrderDetails(saleOrderDetail);

                };
                TblRentOrder? rentOrder = await _rentOrdRepo.Get(model.OrderID);
                if (rentOrder != null)
                {
                    flag = true;
                    if (!rentOrder.Status.Equals(Status.COMPLETED))
                    {
                        result.IsSuccess = false;
                        result.Message = "Đơn hàng chưa hoàn tất";
                        return result;
                    }
                    List<Data.Models.OrderModel.RentOrderDetailResModel> listRentOrderDetail = await _rentOrdDetailRepo.GetRentOrderDetails(rentOrder.Id);
                    TblRentOrderDetail? rentOrderDetail = new();
                    if ((DateTime.Now - (DateTime)rentOrder.CreateDate).TotalDays >= 30)
                    {
                        result.IsSuccess = false;
                        result.Message = "Đã quá hạn để feedback";
                        return result;
                    }
                    foreach (Data.Models.OrderModel.RentOrderDetailResModel i in listRentOrderDetail)
                    {
                        bool check = false;
                        if (model.ProductItemDetailID.Equals(i.ProductItemDetail.Id))
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
                    _ = await _rentOrdDetailRepo.UpdateRentOrderDetail(rentOrderDetail);
                }

                if (flag == false)
                {
                    result.IsSuccess = false;
                    result.Message = "không tìm thấy đơn hàng nào";
                    return result;
                }


                TblFeedBack newFeedback = new()
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
                _ = await _fbRepo.Insert(newFeedback);

                foreach (string url in model.ImagesUrls)
                {
                    TblImage newImg = new()
                    {
                        ImageUrl = url,
                        FeedbackId = newFeedback.Id,
                    };
                    _ = await _imgRepo.Insert(newImg);
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

        public ResultModel getListFeedbackByOrder(string token, PaginationRequestModel pagingModel, Guid orderID)
        {
            ResultModel result = new();
            try
            {

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getListFeedbackByProductItem(string token, PaginationRequestModel pagingModel, Guid productItemID)
        {
            ResultModel result = new();
            try
            {
                List<Data.Models.ProductItemDetailModel.ProductItemDetailResModel> listProductItemDetail = await _proItemDetailRepo.GetSizeProductItems(productItemID, Status.ACTIVE);
                List<FeedbackResModel> listFeedback = new();
                foreach (Data.Models.ProductItemDetailModel.ProductItemDetailResModel i in listProductItemDetail)
                {
                    List<TblFeedBack> listFeedbackRecord = await _fbRepo.GetFeedBackByProductItemDetail(i.Id);
                    foreach (TblFeedBack fb in listFeedbackRecord)
                    {
                        FeedbackResModel fbRes = new();
                        TblUser? user = await _userRepo.Get(fb.UserId);
                        TblSize? size = await _sizeRepo.Get(i.Size.Id);
                        List<string> listImgUrl = await _imgRepo.GetImgUrlFeedback(fb.Id);
                        fbRes.CreateDate = fb.CreateDate;
                        fbRes.UpdateDate = fb.UpdateDate;
                        fbRes.ID = fb.Id;
                        fbRes.Rating = fb.Rating;
                        fbRes.Comment = fb.Comment;
                        fbRes.Status = fb.Status;
                        fbRes.User = new UserCurrResModel
                        {
                            FullName = user.FullName,
                            UserName = user.UserName,
                            Id = user.Id,
                            Phone = user.Phone
                        };
                        fbRes.ProductItemDetail = new Data.Models.ProductItemDetailModel.ProductItemDetailResModel
                        {
                            Id = i.Id,
                            Size = new Data.Models.SizeModel.SizeResModel
                            {
                                Id = size.Id,
                                SizeName = size.Name,
                                SizeType = size.Type
                            }
                        };
                        fbRes.ImageURL = new List<string>();
                        fbRes.ImageURL = listImgUrl;

                        listFeedback.Add(fbRes);
                    }
                }
                PaginationResponseModel pag = new PaginationResponseModel().CurPage(pagingModel.curPage)
                    .PageSize(pagingModel.pageSize)
                    .RecordCount(listFeedback.Count)
                    .PageCount((int)Math.Ceiling((double)listFeedback.Count / pagingModel.pageSize));

                List<FeedbackResModel> pageFeedback = listFeedback.OrderByDescending(y => y.CreateDate)
                    .Skip((pagingModel.curPage - 1) * pagingModel.pageSize)
                    .Take(pagingModel.pageSize).ToList();

                FeedbackRes res = new()
                {
                    FeedbackList = pageFeedback,
                    Paging = pag,
                };

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

        Task<ResultModel> IFeedbackService.getListFeedbackByOrder(string token, PaginationRequestModel pagingModel, Guid orderID)
        {
            throw new NotImplementedException();
        }
    }
}
