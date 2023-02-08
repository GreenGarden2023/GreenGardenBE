using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
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
        private readonly GreenGardenDbContext _context;
        public ProductItemService(/*IMapper mapper,*/ GreenGardenDbContext context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<ResultModel> getAllProductItemByProduct(PaginationRequestModel pagingModel, Guid productId)
        {
            var result = new ResultModel();
            try
            {
                //var listProductItem = _context.TblProductItems.Where(x=>x.)
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
