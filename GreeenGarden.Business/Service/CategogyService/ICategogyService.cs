using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.CategogyService
{
    public interface ICategogyService
    {
        Task<ResultModel> getAllCategories(PaginationRequestModel pagingModel);
        Task<ResultModel> createCategory(string token, CategoryCreateModel categoryCreateModel);
        Task<ResultModel> updateCategory(string token, CategoryUpdateModel categoryUpdateModel);
    }
}
