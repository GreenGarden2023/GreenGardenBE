using System;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Enums;
using System.Security.Claims;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboModel;
using GreeenGarden.Data.Repositories.TakecareComboRepo;
using GreeenGarden.Data.Entities;

namespace GreeenGarden.Business.Service.TakecareComboService
{
	public class TakecareComboService : ITakecareComboService
	{
        private readonly DecodeToken _decodeToken;
        private readonly ITakecareComboRepo _takecareComboRepo;
        public TakecareComboService(ITakecareComboRepo takecareComboRepo)
		{
            _decodeToken = new DecodeToken();
            _takecareComboRepo = takecareComboRepo;
        }
         
        public async Task<ResultModel> GetTakecareComboByID(Guid comboID)
        {
            ResultModel result = new();
            try
            {
                if (comboID == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Combo Id invalid.";
                    return result;
                }
                TblTakecareCombo tblTakecareCombo = await _takecareComboRepo.Get(comboID);
                if (tblTakecareCombo == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Can not find takecare combo with Id: "+comboID+".";
                    return result;
                }
                else
                {
                    TakecareComboModel takecareComboModel = new()
                    {
                        Id = tblTakecareCombo.Id,
                        Name = tblTakecareCombo.Name,
                        Description = tblTakecareCombo.Description,
                        Guarantee = tblTakecareCombo.Guarantee,
                        Price = tblTakecareCombo.Price,
                        Status = tblTakecareCombo.Status,
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = takecareComboModel;
                    result.Message = "Get takece combo success.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> GetTakecareCombos(string status)
        {
            ResultModel result = new();
            try
            {
                
                List<TblTakecareCombo> tblTakecareCombos = await _takecareComboRepo.GetAllTakecareCombo(status);
                List<TakecareComboModel> resList = new List<TakecareComboModel>();
                foreach (var item in tblTakecareCombos)
                {
                    TakecareComboModel takecareComboModel = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Guarantee = item.Guarantee,
                        Price = item.Price,
                        Status = item.Status,
                    };
                    resList.Add(takecareComboModel);
                }
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = resList;
                    result.Message = "Get takecare combos success.";
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

        public async Task<ResultModel> InsertTakecareCombo(TakecareComboInsertModel takecareComboInsertModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
                if (string.IsNullOrEmpty(takecareComboInsertModel.Name))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product name is greater than 1 and shorter than 51 characters";
                    return result;
                }

                TblTakecareCombo tblTakecareCombo = new()
                {
                    Id = Guid.NewGuid(),
                    Name = takecareComboInsertModel.Name,
                    Description = takecareComboInsertModel.Description,
                    Guarantee = takecareComboInsertModel.Guarantee,
                    Price = takecareComboInsertModel.Price,
                };

                Guid insert = await _takecareComboRepo.Insert(tblTakecareCombo);
                if (insert != Guid.Empty)
                {
                    TblTakecareCombo GetTakecareCombo = await _takecareComboRepo.Get(tblTakecareCombo.Id);
                    TakecareComboModel takecareComboModel = new()
                    {
                        Id = GetTakecareCombo.Id,
                        Name = GetTakecareCombo.Name,
                        Description = GetTakecareCombo.Description,
                        Guarantee = GetTakecareCombo.Guarantee,
                        Price = GetTakecareCombo.Price,
                        Status = GetTakecareCombo.Status,
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = takecareComboModel;
                    result.Message = "Insert Takecare combo successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Insert Takecare combo failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> UpdateTakecareCombo(TakecareComboUpdateModel takecareComboUpdateModel, string token)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
                if (takecareComboUpdateModel.Id == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Takecare combo ID invalid.";
                    return result;
                }
                ResultModel update = await _takecareComboRepo.UpdateTakecareCombo(takecareComboUpdateModel);
                if (update.IsSuccess == true)
                {
                    TblTakecareCombo GetTakecareCombo = await _takecareComboRepo.Get(takecareComboUpdateModel.Id);
                    TakecareComboModel takecareComboModel = new()
                    {
                        Id = GetTakecareCombo.Id,
                        Name = GetTakecareCombo.Name,
                        Description = GetTakecareCombo.Description,
                        Guarantee = GetTakecareCombo.Guarantee,
                        Price = GetTakecareCombo.Price,
                        Status = GetTakecareCombo.Status,
                    };
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = takecareComboModel;
                    result.Message = "Update Takecare combo successful.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Data = update.Data;
                    result.Message = "Update Takecare combo failed.";
                    return result;
                }
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

