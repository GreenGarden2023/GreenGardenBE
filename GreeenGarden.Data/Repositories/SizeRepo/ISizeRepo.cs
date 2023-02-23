using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.SizeRepo
{
    public interface ISizeRepo : IRepository<TblSize>
    {
        Task<List<TblSize>> GetProductItemSizes();

    }
}
