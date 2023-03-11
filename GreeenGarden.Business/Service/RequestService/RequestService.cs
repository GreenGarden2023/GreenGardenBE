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

namespace GreeenGarden.Business.Service.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepo _requestRepo;
        private readonly DecodeToken _decodeToken;

        public RequestService(IRequestRepo requestRepo)
        {
            _requestRepo = requestRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> createRequest( RequestModel requestModel, string token)
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
                if(string.IsNullOrEmpty(requestModel.Adress) || string.IsNullOrEmpty(requestModel.Phone) || requestModel.Adress.Length < 8 || requestModel.Adress.Length > 200 || requestModel.Phone.Length < 9 || requestModel.Phone.Length > 11){
                   result.IsSuccess = false;
                   result.Code = 400;
                   result.Message = "Error format address or phone";
                   return result;
                }
                TblRequest newRequest = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = (Guid)requestModel.UserID,
                    Address = requestModel.Adress,
                    Phone = requestModel.Phone,
                    CreateDate = DateTime.Now,
                };
                Guid insertResult = await _requestRepo.Insert(newRequest);
                RequestModel requestReModel = new()
                {
                    ID = newRequest.Id,
                    UserId = (Guid)requestModel.UserID,
                    Address = requestModel.Adress,
                    Phone = requestModel.Phone,
                    CreateDate = DateTime.Now,
                };
            }
            catch (Exception)
            {
           
            }
            
        }
    }
}
