using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Models.RequestModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.RequestRepo;
using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GreeenGarden.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using GreeenGarden.Data.Models.CategoryModel;

namespace GreeenGarden.Business.Service.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepo _requestRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;

        public RequestService(IRequestRepo requestRepo, IImageService imgService, IImageRepo imageRepo)
        {
            _requestRepo = requestRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
        }

        public async Task<ResultModel> createRequest(string token, RequestCreateModel model)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.CUSTOMER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allwed"
                    };
                }

                /*if(string.IsNullOrEmpty(requestModel.Adress) || string.IsNullOrEmpty(requestModel.Phone) || requestModel.Adress.Length < 8 || requestModel.Adress.Length > 200 || requestModel.Phone.Length < 9 || requestModel.Phone.Length > 11){
                   result.IsSuccess = false;
                   result.Code = 400;
                   result.Message = "Error format address or phone";
                   return result;
                }*/
                
                
                TblRequest newRequest = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = model.UserID,
                    Address = model.Adress,
                    Phone = model.Phone,
                    CreateDate = DateTime.Now,
                };
                await _requestRepo.Insert(newRequest);


                foreach (var item in model.requestDetails)
                {
                    var newRequestDetail = new TblRequestDetail() {
                        Id = Guid.NewGuid(),
                        Description = item.Description,
                        TreeName = item.TreeName,
                        Quantity = item.Quantity,
                        Price = 0,
                        RequestId = newRequest.Id,
                        ServiceOrderId = null,
                    };
                    await _requestRepo.CreateRequestDetail(newRequestDetail);

                    foreach (var j in item.Images)
                    {
                        ResultModel? imgUploadUrl = await _imgService.UploadAnImage(j);

                        if (imgUploadUrl != null)
                        {
                            TblImage newimgCategory = new()
                            {
                                Id = Guid.NewGuid(),
                                ImageUrl = imgUploadUrl.Data.ToString(),
                                CategoryId = newRequestDetail.Id,
                            };
                            _ = await _imageRepo.Insert(newimgCategory);

                        }
                    }

                }



                result.IsSuccess = true;
                result.Code = 200;
                result.Data = "";
                result.Message = "Create new request successfully";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
            
        }
    }
}
