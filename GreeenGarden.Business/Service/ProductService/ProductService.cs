using EntityFrameworkPaginateCore;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductService
{
    public class ProductService : IProductService
    {
        //private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;
        public ProductService(/*IMapper mapper,*/ IProductRepo productRepo, IImageService imgService, IImageRepo imageRepo)
        {
            //_mapper = mapper;
            _productRepo= productRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
        }

        public async Task<ResultModel> createProduct(ProductCreateRequestModel model, string token)
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

                var newProduct = new TblProduct()
                {
                    Id = Guid.NewGuid(),
                    Name = model.name,
                    Quantity = 0,
                    Description= model.description,
                    Status = Status.ACTIVE,
                    CategoryId = model.categoryId
                };
                await _productRepo.Insert(newProduct);

                // create, update tbl img()

                var imgUploadUrl = await _imgService.UploadAnImage(model.imgFile);
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
                result.Message = "create new product successfully";
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
                        quantity = p.Quantity,
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
    }
}
