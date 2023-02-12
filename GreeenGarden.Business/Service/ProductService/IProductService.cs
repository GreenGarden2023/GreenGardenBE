using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductService
{
    public interface IProductService
    {
        Task<ResultModel> getAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId);
        Task<ResultModel> createProduct(ProductCreateRequestModel model, string token);
    }
}
