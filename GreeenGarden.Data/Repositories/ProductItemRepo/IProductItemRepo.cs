using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public interface IProductItemRepo
    {
        public Page<TblSubProduct> queryAllSizeByProduct(PaginationRequestModel model, Guid productId);
        public Page<TblProductItem> queryAllItemByProductSize(PaginationRequestModel model, Guid productSizeId);
        public IQueryable<TblProductItem> queryDetailItemByProductSize( Guid productItemId);
        public List<string> getImgByProductItem(Guid productItemId);
    }
}
