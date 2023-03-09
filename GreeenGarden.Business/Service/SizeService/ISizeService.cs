using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeModel;

namespace GreeenGarden.Business.Service.SizeService
{
    public interface ISizeService
    {
        public Task<ResultModel> CreateSize(SizeCreateModel sizeCreateModel, string token);
        public Task<ResultModel> GetSizes();
        public Task<ResultModel> UpdateSizes(SizeUpdateModel model, string token);
        public Task<ResultModel> DeleteSizes(Guid sizeID, string token);
    }
}
