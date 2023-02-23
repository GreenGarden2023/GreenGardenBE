using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeModel;

namespace GreeenGarden.Business.Service.SizeService
{
    public interface ISizeService
    {
        public Task<ResultModel> CreateSize(SizeCreateModel sizeCreateModel, string token);
        public Task<ResultModel> GetSizes();
    }
}
