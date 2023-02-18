using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SubProductModel;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SubProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.SubProductService
{
    public class SubProductService : ISubProductService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ISubProductRepo _subProductRepo;
        public SubProductService(ISubProductRepo subProductRepo)
        {
            _subProductRepo = subProductRepo;
            _decodeToken = new DecodeToken();
        }
        public async Task<ResultModel> createProductSize(SizeItemRequestModel model, string token)
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
                var newSubProduct = new TblSubProduct()
                {
                    Id = Guid.NewGuid(),
                    Name = model.name,
                    MinPrice = model.minPrice,
                    Price= model.price,
                    MaxPrice= model.maxPrice,
                    ProductId = model.productId,
                    SizeId = model.sizeId,
                    Quantity = 0
                };
                await _subProductRepo.Insert(newSubProduct);

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Create new category successfully";
                return result;

                // Code
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
