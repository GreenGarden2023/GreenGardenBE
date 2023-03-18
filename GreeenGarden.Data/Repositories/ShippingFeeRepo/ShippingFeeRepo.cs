using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ShippingFeeModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ShippingFeeRepo
{
	public class ShippingFeeRepo : Repository<TblShippingFee>, IShippingFeeRepo
	{
		private readonly GreenGardenDbContext _context;
		public ShippingFeeRepo(GreenGardenDbContext context) : base(context)
		{
			_context = context;
		}
        public async Task<List<TblShippingFee>> GetListShipingFee()
        {
			List<TblShippingFee> tblShippingFees = await _context.TblShippingFees.ToListAsync();
			return tblShippingFees;
        }

        public async Task<TblShippingFee> GetShippingFeeByDistrict(int districtID)
        {
            TblShippingFee tblShippingFees = await _context.TblShippingFees.Where(x => x.DistrictId.Equals(districtID)).FirstOrDefaultAsync();
            return tblShippingFees;
        }

        public async Task<ResultModel> UpdateShippingFee(ShippingFeeInsertModel shippingFeeInsertModel)
        {
			ResultModel result = new ResultModel();
			try
			{
				TblShippingFee tblShippingFee = await _context.TblShippingFees.Where(x => x.DistrictId.Equals(shippingFeeInsertModel.DistrictID)).FirstOrDefaultAsync();
				if(tblShippingFee != null)
				{
					tblShippingFee.FeeAmount = shippingFeeInsertModel.FeeAmount;
					_ = _context.Update(tblShippingFee);
					_ = await _context.SaveChangesAsync();
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Message = "Update shipping fee success.";
                    return result;
				}
				else
				{
                    result.Code = 400;
                    result.IsSuccess = false;
					result.Message = "Update shipping fee for " + shippingFeeInsertModel.DistrictID + " failed.";
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
    }
}

