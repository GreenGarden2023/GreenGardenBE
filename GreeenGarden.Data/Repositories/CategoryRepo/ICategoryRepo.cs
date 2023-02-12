using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserModels;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.CategoryRepo
{
    public interface ICategoryRepo : IRepository<TblCategory>
    {
        public Page<TblCategory> queryAllCategories(PaginationRequestModel pagingModel);
        public string getImgByCategory(Guid categoryId);
        public TblCategory selectDetailCategory(Guid categoryId);
    }
}
