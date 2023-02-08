using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public interface IProductItemService
    {
        Task<ResultModel> getAllProductItemByProductItemSize(PaginationRequestModel pagingModel, Guid productSizeId);
        Task<ResultModel> getSizesOfProduct(PaginationRequestModel pagingModel, Guid productId);
        Task<ResultModel> getDetailItem(Guid productItemId);
    }
}
