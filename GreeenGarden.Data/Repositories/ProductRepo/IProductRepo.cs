using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductRepo
{
    public interface IProductRepo : IRepository<TblProduct>
    {
        public Page<TblProduct> queryAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId);
        public string getImgByProduct(Guid productId);
    }
}
