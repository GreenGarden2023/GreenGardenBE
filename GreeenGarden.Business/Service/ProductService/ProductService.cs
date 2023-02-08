using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductService
{
    public class ProductService : IProductService
    {
        //private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        public ProductService(/*IMapper mapper,*/ IProductRepo productRepo)
        {
            //_mapper = mapper;
            _productRepo= productRepo;
        }
        public async Task<ResultModel> getAllProductByCategory(PaginationRequestModel pagingModel, Guid categoryId)
        {
            var result = new ResultModel();
            try
            {
                if (categoryId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Need categoryId to get product";
                    return result;
                }
                var listProdct = _productRepo.queryAllProductByCategory(pagingModel, categoryId);

                if (listProdct == null)
                {
                    result.Message = "List null";
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listProdct;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
