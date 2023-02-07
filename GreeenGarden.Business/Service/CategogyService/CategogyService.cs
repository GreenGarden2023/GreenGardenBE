using AutoMapper;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
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
        private readonly GreenGardenDbContext _context;
        public CategogyService(/*IMapper mapper,*/ GreenGardenDbContext context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<ResultModel> getAllCategories(PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
            {
                var listCategories = _context.TblCategories.Where(x=> x.Status == "active").ToList();

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
