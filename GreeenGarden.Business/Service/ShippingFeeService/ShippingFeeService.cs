using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ShippingFeeModel;
using GreeenGarden.Data.Repositories.ShippingFeeRepo;

namespace GreeenGarden.Business.Service.ShippingFeeService
{
	public class ShippingFeeService : IShippingFeeService
	{
        private readonly IShippingFeeRepo _shippingFeeRepo;
		public ShippingFeeService(IShippingFeeRepo shippingFeeRepo )
		{
            _shippingFeeRepo = shippingFeeRepo;
		}

        public async Task<ResultModel> GetListShipingFee()
        {
            ResultModel result = new ResultModel();
            try
            {
                List<ShippingFeeResModel> resList = new List<ShippingFeeResModel>();
                List<TblShippingFee> getList = await _shippingFeeRepo.GetListShipingFee();
                if (getList != null && getList.Any())
                {
                    foreach(TblShippingFee shippingFee in getList)
                    {
                        ShippingFeeResModel resModel = new ShippingFeeResModel
                        {
                            ID = shippingFee.Id,
                            District = shippingFee.District,
                            FeeAmount = shippingFee.FeeAmount
                        };
                        resList.Add(resModel);
                    }
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resList;
                    result.Message = "Get list success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "List Empty.";
                    return result;
                }
            }catch(Exception e)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = e.ToString();
                return result;
            }
        }

        public async Task<ResultModel> UpdateShippingFee(List<ShippingFeeInsertModel> shippingFeeInsertModels)
        {
            ResultModel result = new ResultModel();
            try
            {
                bool successAll = false;
                foreach (ShippingFeeInsertModel model in shippingFeeInsertModels)
                {
                    ResultModel update = await _shippingFeeRepo.UpdateShippingFee(model);
                    if (update.IsSuccess)
                    {
                        successAll = true;
                    }
                    else
                    {
                        successAll = false;
                    }
                }
                if(successAll == true)
                {
                    List<ShippingFeeResModel> resList = new List<ShippingFeeResModel>();
                    foreach (ShippingFeeInsertModel shippingFee in shippingFeeInsertModels)
                    {
                        TblShippingFee tblShippingFee = await _shippingFeeRepo.GetAShippingFee(shippingFee.ID);
                        ShippingFeeResModel resModel = new ShippingFeeResModel
                        {
                            ID = tblShippingFee.Id,
                            District = tblShippingFee.District,
                            FeeAmount = tblShippingFee.FeeAmount
                        };
                        resList.Add(resModel);
                    }
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resList;
                    result.Message = "Update shipping fee success";
                    return result;
                }
                else{
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update shipping fee failed"; ;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = e.ToString();
                return result;
            }
        }
    }
}

