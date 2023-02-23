using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.SizeRepo;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.SizeService
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepo _sizeRepo;
        private readonly DecodeToken _decodeToken;
        public SizeService(ISizeRepo sizeRepo)
        {
            _sizeRepo = sizeRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> CreateSize(SizeCreateModel sizeCreateModel, string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }

                if (String.IsNullOrEmpty(sizeCreateModel.SizeName) || sizeCreateModel.SizeName.Length < 2) return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "SizeName Invalid"
                };
                var newSize = new TblSize()
                {
                    Id = Guid.NewGuid(),
                    Name = sizeCreateModel.SizeName
                };
                await _sizeRepo.Insert(newSize);
                var newSizeRes = new SizeModel()
                {
                    Id = newSize.Id,
                    SizeName = newSize.Name
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = newSizeRes;
                result.Message = "Create new size successfully";
                return result;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetSizes()
        {
            var sizes = await _sizeRepo.GetProductItemSizes();
            List<SizeModel> sizeModels = new();
            foreach(TblSize size in sizes)
            {
                var viewSize = new SizeModel()
                {
                    Id = size.Id,
                    SizeName = size.Name
                };
                sizeModels.Add(viewSize);
            }
            var result = new ResultModel();
            result.IsSuccess = true;
            result.Code = 200;
            result.Data = sizeModels;
            return result;
        }
    }
}
