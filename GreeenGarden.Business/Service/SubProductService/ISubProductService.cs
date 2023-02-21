using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SubProductModel;

namespace GreeenGarden.Business.Service.SubProductService
{
    public interface ISubProductService
    {
        public Task<ResultModel> createProductSize(SizeItemRequestModel model, string token);
        public Task<ResultModel> getProductSize(Guid productId);
    }
}
