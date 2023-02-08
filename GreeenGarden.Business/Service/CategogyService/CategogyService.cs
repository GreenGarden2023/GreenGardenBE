using AutoMapper;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.CategogyService
{
    public class CategogyService : ICategogyService
    {
        //private readonly IMapper _mapper;
        private readonly ICategoryRepo _cateRepo;
        public CategogyService(/*IMapper mapper,*/ ICategoryRepo cateRepo)
        {
            //_mapper = mapper;
            _cateRepo = cateRepo;
        }
        public async Task<ResultModel> getAllCategories(PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
            {
                var listCategories = _cateRepo.queryAllCategories(pagingModel);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listCategories;
            }
            catch (Exception e )
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
