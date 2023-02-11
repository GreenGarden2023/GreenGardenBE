using AutoMapper;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.CategogyService
{
    public class CategogyService : ICategogyService
    {
        //private readonly IMapper _mapper;
        private readonly ICategoryRepo _cateRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imgRepo;

        public CategogyService(/*IMapper mapper,*/ ICategoryRepo cateRepo, IImageService imgService)
        {
            //_mapper = mapper;
            _cateRepo = cateRepo;
            _decodeToken = new DecodeToken(); 
            _imgService = imgService;
        }

        public async Task<ResultModel> createCategory(string token, string nameCategory, IFormFile file)
        {
            var result = new ResultModel();
            try
            {
                
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                var newCategory = new TblCategory()
                {
                    Name = nameCategory,
                    Id = Guid.NewGuid(),
                    Status = Status.ACTIVE
                };



                IList<IFormFile> listImg = null;
                listImg.Add(file);
                var checkUploadImg = await _imgService.UploadImage(listImg);
                if (checkUploadImg.IsSuccess == true)
                {
                    var newImg = new TblImage()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = checkUploadImg.Data.ToString(),
                        CategoryId = newCategory.Id
                    };
                    await _imgRepo.Insert(newImg);
                }


                await _cateRepo.Insert(newCategory);

                result.IsSuccess = true;
                result.Message = "create new category successfully";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            throw new NotImplementedException();
        }

        public async Task<ResultModel> getAllCategories(PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
            {
                var listCategories = _cateRepo.queryAllCategories(pagingModel);
                if (listCategories == null)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listCategories;
                    result.Message = "null";
                    return result;
                }

                List<CategoryModel> dataList = new List<CategoryModel>();
                foreach (var c in listCategories.Results)
                {
                    var CategoryToShow = new CategoryModel
                    {
                        id = c.Id,
                        name = c.Name,
                        status = c.Status,
                        imgUrl = "" + _cateRepo.getImgByCategory(c.Id)
                    };
                    dataList.Add(CategoryToShow);
                }
                var paging = new PaginationResponseModel()
                    
                    .PageSize(listCategories.PageSize)
                    .CurPage(listCategories.CurrentPage)
                    .RecordCount(listCategories.RecordCount)
                    .PageCount(listCategories.PageCount);


                var response = new ResponseResult()
                {
                    Paging = paging,
                    Result = dataList
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = response;
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
