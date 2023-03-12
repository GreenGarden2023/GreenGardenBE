using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using System.Globalization;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.CategogyService
{
    public class CategogyService : ICategogyService
    {
        private readonly ICategoryRepo _cateRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;

        public CategogyService(ICategoryRepo cateRepo, IImageService imgService, IImageRepo imageRepo)
        {
            _cateRepo = cateRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
        }

        public async Task<ResultModel> changeStatus(string token, CategoryUpdateStatusModel model)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }


                result.IsSuccess = await _cateRepo.updateStatus(model);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> createCategory(string token, CategoryCreateModel categoryCreateModel)
        {
            ResultModel result = new();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF))
            {
                result.Code = 403;
                result.IsSuccess = false;
                result.Message = "User role invalid";
                return result;
            }


            if (string.IsNullOrEmpty(categoryCreateModel.Name) || categoryCreateModel.Name.Length < 2 || categoryCreateModel.Name.Length > 50)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category name is greater than 2 and less than 50 characters";
                return result;
            }
            if (_cateRepo.checkCategoryNameExist(categoryCreateModel.Name))
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category name duplicated";
                return result;
            }
            if (categoryCreateModel.Image == null)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category image not found";
                return result;
            }
            try
            {


                //Insert Category
                TblCategory newCategory = new()
                {
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(categoryCreateModel.Name.ToLower()),
                    Id = Guid.NewGuid(),
                    Status = Status.ACTIVE,
                    Description = categoryCreateModel.Description
                };
                _ = await _cateRepo.Insert(newCategory);

                ResultModel? imgUploadUrl = await _imgService.UploadAnImage(categoryCreateModel.Image);

                if (imgUploadUrl != null)
                {
                    TblImage newimgCategory = new()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = imgUploadUrl.Data.ToString(),
                        CategoryId = newCategory.Id,
                    };
                    _ = await _imageRepo.Insert(newimgCategory);

                }
                CategoryModel categoryToShow = new()
                {
                    Id = newCategory.Id,
                    Name = newCategory.Name,
                    Status = newCategory.Status,
                    ImgUrl = imgUploadUrl.Data.ToString(),
                    Description = newCategory.Description
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
            ResultModel result = new();
            try
            {
                EntityFrameworkPaginateCore.Page<TblCategory>? listCategories = await _cateRepo.GetAllCategory(pagingModel);
                if (listCategories == null)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listCategories;
                    result.Message = "null";
                    return result;
                }

                List<CategoryModel> dataList = new();
                foreach (TblCategory c in listCategories.Results)
                {
                    CategoryModel CategoryToShow = new()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Status = c.Status,
                        ImgUrl = "" + _cateRepo.getImgByCategory(c.Id),
                        Description = c.Description

                    };
                    dataList.Add(CategoryToShow);
                }
                PaginationResponseModel paging = new PaginationResponseModel()

                    .PageSize(listCategories.PageSize)
                    .CurPage(listCategories.CurrentPage)
                    .RecordCount(listCategories.RecordCount)
                    .PageCount(listCategories.PageCount);


                ResponseResult response = new()
                {
                    Paging = paging,
                    Result = dataList
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = response;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCategoryByStatus(PaginationRequestModel pagingModel, string status)
        {
            ResultModel result = new();
            try
            {
                EntityFrameworkPaginateCore.Page<TblCategory>? listCategories = await _cateRepo.GetCategoryByStatus(pagingModel, status);
                if (listCategories == null)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listCategories;
                    result.Message = "null";
                    return result;
                }

                List<CategoryModel> dataList = new();
                foreach (TblCategory c in listCategories.Results)
                {
                    CategoryModel CategoryToShow = new()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Status = c.Status,
                        ImgUrl = "" + _cateRepo.getImgByCategory(c.Id),
                        Description = c.Description

                    };
                    dataList.Add(CategoryToShow);
                }
                PaginationResponseModel paging = new PaginationResponseModel()

                    .PageSize(listCategories.PageSize)
                    .CurPage(listCategories.CurrentPage)
                    .RecordCount(listCategories.RecordCount)
                    .PageCount(listCategories.PageCount);


                ResponseResult response = new()
                {
                    Paging = paging,
                    Result = dataList
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = response;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> updateCategory(string token, CategoryUpdateModel categoryUpdateModel)
        {
            ResultModel result = new();
            string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
            if (!userRole.Equals(Commons.MANAGER)
                && !userRole.Equals(Commons.STAFF))
            {
                result.Code = 403;
                result.IsSuccess = false;
                result.Message = "User role invalid";
                return result;
            }
            if (string.IsNullOrEmpty(categoryUpdateModel.Name) &&
                string.IsNullOrEmpty(categoryUpdateModel.Description) &&
                string.IsNullOrEmpty(categoryUpdateModel.Status) &&
                 categoryUpdateModel.Image == null)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Please update atleast 1 parameter";
                return result;
            }

            if (string.IsNullOrEmpty(categoryUpdateModel.Name) || categoryUpdateModel.Name.Length < 2 || categoryUpdateModel.Name.Length > 50)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category name is greater than 2 and less than 50 characters";
                return result;
            }

            if (string.IsNullOrEmpty(categoryUpdateModel.Status) || categoryUpdateModel.Status.Length < 2 || categoryUpdateModel.Status.Length > 50)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Category status is greater than 2 and less than 50 characters";
                return result;
            }


            try
            {
                TblCategory categoryUpdate = await _cateRepo.updateCategory(categoryUpdateModel);
                if (categoryUpdate == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Can not find category with ID: " + categoryUpdateModel.ID;
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Category updated without image change";
                if (categoryUpdate != null && categoryUpdateModel.Image != null)
                {
                    ResultModel imgUpdate = await _imgService.UpdateImageCategory(categoryUpdateModel.ID, categoryUpdateModel.Image);
                    if (imgUpdate != null)
                    {
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
