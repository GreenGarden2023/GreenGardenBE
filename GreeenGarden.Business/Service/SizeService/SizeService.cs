using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
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

        public async Task<ResultModel> createSize(string sizeName, string token)
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
                        Message = "User not allowed"
                    };
                }

                if (sizeName == null) return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "SizeName not null"
                };
                var newSize = new TblSize()
                {
                    Id = Guid.NewGuid(),
                    Name = sizeName
                };
                await _sizeRepo.Insert(newSize);

                result.IsSuccess = true;
                result.Message = "create new size successfully";
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
    }
}
