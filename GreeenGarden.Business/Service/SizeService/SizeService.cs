﻿using GreeenGarden.Business.Utilities.TokenService;
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
            ResultModel result = new();
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

                if (string.IsNullOrEmpty(sizeCreateModel.SizeName) || sizeCreateModel.SizeName.Length < 2)
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "SizeName Invalid"
                    };
                }

                TblSize newSize = new()
                {
                    Id = Guid.NewGuid(),
                    Type = sizeCreateModel.SizeType,
                    Name = sizeCreateModel.SizeName
                };
                _ = await _sizeRepo.Insert(newSize);
                SizeResModel newSizeRes = new()
                {
                    Id = newSize.Id,
                    SizeType = newSize.Type,
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

        public async Task<ResultModel> DeleteSizes(Guid sizeID, string token)
        {
            ResultModel result = new();
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

                bool res = await _sizeRepo.DeleteSizes(sizeID);
                if (!res)
                {
                    result.IsSuccess = false;
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Message = "Delete successfully";
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
            List<TblSize> sizes = await _sizeRepo.GetProductItemSizes();
            List<SizeResModel> sizeModels = new();
            foreach (TblSize size in sizes)
            {
                SizeResModel viewSize = new()
                {
                    Id = size.Id,
                    SizeType = size.Type,
                    SizeName = size.Name
                };
                sizeModels.Add(viewSize);
            }
            ResultModel result = new()
            {
                IsSuccess = true,
                Code = 200,
                Data = sizeModels
            };
            return result;
        }

        public async Task<ResultModel> UpdateSizes(SizeUpdateModel model, string token)
        {
            ResultModel result = new();
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

                bool res = await _sizeRepo.UpdateSizes(model);
                if (!res)
                {
                    result.IsSuccess = false;
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = model;
                result.Message = "Update successfully";
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
