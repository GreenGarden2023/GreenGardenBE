using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ShippingFeeModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ShippingFeeRepo
{
    public interface IShippingFeeRepo : IRepository<TblShippingFee>
    {
        Task<ResultModel> UpdateShippingFee(ShippingFeeInsertModel shippingFeeInsertModel);
        Task<List<TblShippingFee>> GetListShipingFee();
        Task<TblShippingFee> GetShippingFeeByDistrict(int districtID);
    }
}

