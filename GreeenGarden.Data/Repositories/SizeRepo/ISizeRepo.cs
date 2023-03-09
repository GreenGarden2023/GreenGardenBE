using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SizeRepo
{
    public interface ISizeRepo : IRepository<TblSize>
    {
        Task<List<TblSize>> GetProductItemSizes();
        Task<bool> UpdateSizes(SizeUpdateModel model);
        Task<bool> DeleteSizes(Guid sizeID);

    }
}
