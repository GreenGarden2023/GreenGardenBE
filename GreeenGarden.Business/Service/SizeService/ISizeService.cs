using GreeenGarden.Data.Models.ResultModel;

namespace GreeenGarden.Business.Service.SizeService
{
    public interface ISizeService
    {
        public Task<ResultModel> createSize(string sizeName, string token);
    }
}
