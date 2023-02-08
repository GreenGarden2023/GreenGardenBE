using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public class ProductItemService : IProductItemService
    {
        //private readonly IMapper _mapper;
        private readonly IProductItemRepo _proItemRepo;
        public ProductItemService(/*IMapper mapper,*/ IProductItemRepo proItemRepo)
        {
            //_mapper = mapper;
            _proItemRepo= proItemRepo;
        }

        public async Task<ResultModel> getAllProductItemByProductItemSize(PaginationRequestModel pagingModel, Guid productSizeId)
        {
            var result = new ResultModel();
            try
            {
                var listItemBySize = _proItemRepo.queryAllItemByProductSize(pagingModel, productSizeId);
                if (listItemBySize == null)
                {
                    result.Message = "Don't have any item in size!";
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listItemBySize;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getAllProductItemSizeByProduct(PaginationRequestModel pagingModel, Guid productId)
        {
            var result = new ResultModel();
            try
            {
                var listProductSize = _proItemRepo.queryAllSizeByProduct(pagingModel, productId);
                if (listProductSize == null)
                {
                    result.Message = "Don't have any size of this product";
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listProductSize;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getDetailItem(Guid productItemId)
        {
            var result = new ResultModel();
            try
            {
                var productItem = _proItemRepo.queryDetailItemByProductSize(productItemId);
                if (productItem == null)
                {
                    result.Message = "Item does not exist!";
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = productItem;
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
