using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.ProductService
{
    public class ProductService : IProductService
    {
        //private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;
        private readonly ICategoryRepo _categoryRepo;
        public ProductService(/*IMapper mapper,*/ IProductRepo productRepo, IImageService imgService, IImageRepo imageRepo, ICategoryRepo categoryRepo)
        {
            //_mapper = mapper;
            _productRepo = productRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<ResultModel> createProduct(ProductCreateModel model, string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
                if (String.IsNullOrEmpty(model.Name) || model.Name.Length < 2 || model.Name.Length > 50)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product name is greater than 1 and shorter than 51 characters";
                    return result;
                }
                if (model.ImgFile == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product not found";
                    return result;
                }
                var newProduct = new TblProduct()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Status = Status.ACTIVE,
                    CategoryId = model.CategoryId
                };
                var insertResult = await _productRepo.Insert(newProduct);

                // create, update tbl img()

                var imgUploadUrl = await _imgService.UploadAnImage(model.ImgFile);
                if (imgUploadUrl != null)
                {
                    var newimgCategory = new TblImage()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = imgUploadUrl.Data.ToString(),
                        ProductId = newProduct.Id,
                    };
                    await _imageRepo.Insert(newimgCategory);

                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Create new product successfully";
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
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listProdct;
                    return result;
                }

                List<ProductModel> dataList = new List<ProductModel>();
                foreach (var p in listProdct.Results)
                {
                    var productToShow = new ProductModel
                    {
                        id = p.Id,
                        name = p.Name,
                        description = p.Description,
                        status = p.Status,
                        categoryId = p.CategoryId,
                        imgUrl = _productRepo.getImgByProduct(p.Id),
                    };
                    dataList.Add(productToShow);
                }
                var paging = new PaginationResponseModel()
                    .PageSize(listProdct.PageSize)
                    .CurPage(listProdct.CurrentPage)
                    .RecordCount(listProdct.RecordCount)
                    .PageCount(listProdct.PageCount);


                var response = new ResponseResult()
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
        public async Task<ResultModel> getAllProduct(PaginationRequestModel pagingModel)
        {
            var result = new ResultModel();
            try
            {

                var listProdct = _productRepo.queryAllProduct(pagingModel);

                if (listProdct == null)
                {
                    result.Message = "List null";
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listProdct;
                    return result;
                }

                List<ProductModel> dataList = new List<ProductModel>();
                foreach (var p in listProdct.Results)
                {
                    var productToShow = new ProductModel
                    {
                        id = p.Id,
                        name = p.Name,
                        description = p.Description,
                        status = p.Status,
                        categoryId = p.CategoryId,
                        imgUrl = _productRepo.getImgByProduct(p.Id),
                    };
                    dataList.Add(productToShow);
                }
                var paging = new PaginationResponseModel()
                    .PageSize(listProdct.PageSize)
                    .CurPage(listProdct.CurrentPage)
                    .RecordCount(listProdct.RecordCount)
                    .PageCount(listProdct.PageCount);


                var response = new ResponseResult()
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

        public async Task<ResultModel> UpdateProduct(ProductUpdateModel productUpdateModel, string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
                if (String.IsNullOrEmpty(productUpdateModel.Name) &&
                String.IsNullOrEmpty(productUpdateModel.Description) &&
                String.IsNullOrEmpty(productUpdateModel.Status) &&
                productUpdateModel.CategoryId == Guid.Empty &&
                 productUpdateModel.Image == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Please update atleast 1 parameter";
                    return result;
                }

                if (String.IsNullOrEmpty(productUpdateModel.Name) || productUpdateModel.Name.Length < 2 || productUpdateModel.Name.Length > 50)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Product name is greater than 2 and less than 50 characters";
                    return result;
                }

                if (String.IsNullOrEmpty(productUpdateModel.Status) || productUpdateModel.Status.Length < 2 || productUpdateModel.Status.Length > 50)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Product status is greater than 2 and less than 50 characters";
                    return result;
                }
                if (!_categoryRepo.checkCategoryIDExist(productUpdateModel.CategoryId))
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Cannot find category with ID: " + productUpdateModel.CategoryId;
                    return result;
                }

                var productUpdate = await _productRepo.UpdateProduct(productUpdateModel);

                if (productUpdate == false)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Can not find product with ID: " + productUpdateModel.ID;
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Product updated without image change";
                if (productUpdate != false && productUpdateModel.Image != null)
                {
                    var productImgUpdate = await _imgService.UpdateImageProduct(productUpdateModel.ID, productUpdateModel.Image);
                    if (productImgUpdate != null)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Product updated with image change";
                    }
                }
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
    }
}
