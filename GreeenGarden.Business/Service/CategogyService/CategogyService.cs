using AutoMapper;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.ImgUtility;
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.CategogyService
{
    public class CategogyService : ICategogyService
    {
        private readonly ICategoryRepo _cateRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
       private readonly IImageRepo _imageRepo;

        public CategogyService( ICategoryRepo cateRepo, IImageService imgService, IImageRepo imageRepo)
        {
            _cateRepo = cateRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
        }

        public async Task<ResultModel> createCategory(string token, CategoryCreateModel categoryCreateModel)
        {
            var result = new ResultModel();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF))
            {
                result.Code = 403;
                result.IsSuccess = false;
                result.Message = "User role invalid";
                return result;
            }
            if (String.IsNullOrEmpty(categoryCreateModel.Name))
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category name cannot be empty.";
                return result;
            }
            if (categoryCreateModel.Image == null)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category image not found.";
                return result;
            }
            try
            {
                
                
                //Insert Category
                var newCategory = new TblCategory()
                {
                    Name = categoryCreateModel.Name,
                    Id = Guid.NewGuid(),
                    Status = Status.ACTIVE,
                    Description = categoryCreateModel.Description
                };
                await _cateRepo.Insert(newCategory);

                var imgUploadUrl = await _imgService.UploadAnImage(categoryCreateModel.Image);

                if (imgUploadUrl != null)
                {
                    var newimgCategory = new TblImage()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = imgUploadUrl.Data.ToString(),
                        CategoryId = newCategory.Id,
                    };
                    await _imageRepo.Insert(newimgCategory);

                }
                var categoryToShow = new CategoryModel()
                {
                    id = newCategory.Id,
                    name = newCategory.Name,
                    status = newCategory.Status,
                    imgUrl = imgUploadUrl.Data.ToString()
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = categoryToShow;
                result.Message = "Create new category successfully";
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }

            
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

        public async Task<ResultModel> updateCategory(string token, CategoryUpdateModel categoryUpdateModel)
        {
            var result = new ResultModel();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF))
            {
                result.Code = 403;
                result.IsSuccess = false;
                result.Message = "User role invalid";
                return result;
            }
            if (String.IsNullOrEmpty(categoryUpdateModel.Name) &&
                String.IsNullOrEmpty(categoryUpdateModel.Description) &&
                String.IsNullOrEmpty(categoryUpdateModel.Status) &&
                 categoryUpdateModel.Image == null) {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Please update atleast 1 parameter";
                return result;
            }


            if (categoryUpdateModel == null)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category invalid.";
                return result;
            }
            try
            {
                var categoryUpdate = await _cateRepo.updateCategory(categoryUpdateModel);
                if (categoryUpdate == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Can not find category with ID: "+ categoryUpdateModel.ID;
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Category updated without image change";
                if (categoryUpdate != null && categoryUpdateModel.Image != null)
                {
                    var imgUpdate = await _imgService.UpdateImageCategory(categoryUpdateModel.ID, categoryUpdateModel.Image);
                    if(imgUpdate != null)
                    {
                        var categoryToShow = new CategoryModel()
                        {
                            id = categoryUpdateModel.ID,
                            name = categoryUpdateModel.Name,
                            status = categoryUpdateModel.Status,
                            imgUrl = imgUpdate.Data.ToString(),
                        };

                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Category updated with image change";
                    }
                }
                return result;

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
