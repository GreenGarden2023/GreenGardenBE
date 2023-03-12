using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.RequestModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.RequestRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IRequestRepo _requestRepo;

        public RequestService(IRequestRepo requestRepo)
        {
            _requestRepo = requestRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> ChangeStatus(string token, RequestUpdateStatusModel model)
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
                result.IsSuccess = await _requestRepo.changeStatus(model);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateRequest(string token, RequestCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _requestRepo.GetUserByUsername(_decodeToken.Decode(token, "username"));
                var newRequest = new TblRequest()
                {
                    Id = Guid.NewGuid(),
                    UserId= tblUser.Id,
                    Address = model.Address,
                     Phone= model.Phone,
                     CreateDate= DateTime.Now,
                };
                await _requestRepo.Insert(newRequest);

                foreach (var i in model.RequestDetail)
                {
                    var newRequestDetail = new TblRequestDetail()
                    {
                        Id = Guid.NewGuid() ,
                        TreeName = i.TreeName,
                        Quantity= i.Quantity,
                        Description= i.Description,
                        RequestId =  newRequest.Id,
                        ServiceOrderId =null,
                        Price = null,
                    };
                    await _requestRepo.InsertRequestDetail(newRequestDetail);
                    foreach (var j in i.ImageUrl)
                    {
                        var newImg = new TblImage()
                        {
                            Id= Guid.NewGuid(),
                            ImageUrl = j,
                            RequestDetailId = newRequestDetail.Id,
                        };
                        await _requestRepo.InsertImage(newImg);
                    }
                }

                result.Code = 201;
                result.IsSuccess = true;
                result.Message ="Create succesfully!";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetListRequest(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _requestRepo.GetUserByUsername(_decodeToken.Decode(token, "username"));

                var res = await _requestRepo.GetListRequest(tblUser.Id);

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
