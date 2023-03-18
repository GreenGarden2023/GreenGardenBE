using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ShippingFeeModel;

namespace GreeenGarden.Business.Service.ShippingFeeService
{
	public interface IShippingFeeService
	{
        Task<ResultModel> GetListShipingFee();
        Task<ResultModel> UpdateShippingFee(List<ShippingFeeInsertModel> shippingFeeInsertModels);
    }
}

