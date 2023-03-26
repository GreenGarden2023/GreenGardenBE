using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ShippingFeeModel;
using GreeenGarden.Data.Repositories.DistrictRepo;
using GreeenGarden.Data.Repositories.ShippingFeeRepo;

namespace GreeenGarden.Business.Service.ShippingFeeService
{
    public class ShippingFeeService : IShippingFeeService
    {
        private readonly IShippingFeeRepo _shippingFeeRepo;
        private readonly IDistrictRepo _districtRepo;
        public ShippingFeeService(IShippingFeeRepo shippingFeeRepo, IDistrictRepo districtRepo)
        {
            _shippingFeeRepo = shippingFeeRepo;
            _districtRepo = districtRepo;
        }

        public async Task<ResultModel> GetListShipingFee()
        {
            ResultModel result = new();
            try
            {
                List<ShippingFeeResModel> resList = new();
                List<TblDistrict> districtList = await _districtRepo.GetDistrictList();
                if (districtList != null && districtList.Any())
                {
                    foreach (TblDistrict district in districtList)
                    {
                        TblShippingFee fee = await _shippingFeeRepo.GetShippingFeeByDistrict(district.Id);
                        ShippingFeeResModel resModel = new()
                        {
                            DistrictID = district.Id,
                            District = district.DistrictName,
                            FeeAmount = fee.FeeAmount
                        };
                        resList.Add(resModel);
                    }
                    resList.Sort((x, y) => x.DistrictID.CompareTo(y.DistrictID));
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resList;
                    result.Message = "Get list success.";
                    return result;
                }
                else
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = districtList;
                    result.Message = "List Empty.";
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

        public async Task<ResultModel> UpdateAnShippingFee(ShippingFeeInsertModel shippingFeeInsertModels)
        {
            ResultModel result = new();
            bool shippingIDCheck = false;
            for (int i = 1; i <= 19; i++)
            {
                if (shippingFeeInsertModels.DistrictID == i)
                {
                    shippingIDCheck = true;
                }
            }
            if (shippingIDCheck == false)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "District ID invalid.";
                return result;
            }
            try
            {

                ResultModel update = await _shippingFeeRepo.UpdateShippingFee(shippingFeeInsertModels);
                if (update.IsSuccess)
                {
                    List<ShippingFeeResModel> resList = new();

                    TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(shippingFeeInsertModels.DistrictID);
                    ShippingFeeResModel resModel = new()
                    {
                        DistrictID = tblShippingFee.DistrictId,
                        District = "" + await _districtRepo.GetADistrict(shippingFeeInsertModels.DistrictID),
                        FeeAmount = tblShippingFee.FeeAmount
                    };

                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resModel;
                    result.Message = "Update shipping fee success";
                    return result;
                }
                else
                {
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

        public async Task<ResultModel> UpdateShippingFee(List<ShippingFeeInsertModel> shippingFeeInsertModels)
        {
            ResultModel result = new();
            try
            {
                bool successAll = false;
                foreach (ShippingFeeInsertModel model in shippingFeeInsertModels)
                {
                    bool shippingIDCheck = false;
                    for (int i = 1; i <= 19; i++)
                    {
                        if (model.DistrictID == i)
                        {
                            shippingIDCheck = true;
                        }
                    }
                    if (shippingIDCheck == false)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "District ID invalid.";
                        return result;
                    }
                    ResultModel update = await _shippingFeeRepo.UpdateShippingFee(model);
                    successAll = update.IsSuccess;
                }
                if (successAll == true)
                {
                    List<ShippingFeeResModel> resList = new();
                    foreach (ShippingFeeInsertModel shippingFee in shippingFeeInsertModels)
                    {
                        TblShippingFee tblShippingFee = await _shippingFeeRepo.GetShippingFeeByDistrict(shippingFee.DistrictID);
                        ShippingFeeResModel resModel = new()
                        {
                            DistrictID = tblShippingFee.DistrictId,
                            District = "" + await _districtRepo.GetADistrict(shippingFee.DistrictID),
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
                else
                {
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

