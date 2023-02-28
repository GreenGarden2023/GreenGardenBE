using System.Security.Claims;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Data.Repositories.SubProductRepo;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepo _proItemRepo;
        private readonly IImageService _imgService;
        private readonly DecodeToken _decodeToken;
        private readonly IImageRepo _imageRepo;
        private readonly IProductRepo _proRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ISizeRepo _sizeRepo;
        public ProductItemService(ISizeRepo sizeRepo, IProductItemRepo proItemRepo, IProductRepo proRepo, IImageRepo imageRepo, IImageService imgService, ICategoryRepo categoryRepo)
        {
            _proItemRepo = proItemRepo;
            _imgService = imgService;
            _decodeToken = new DecodeToken();
            _imageRepo = imageRepo;
            _proRepo = proRepo;
            _categoryRepo = categoryRepo;
            _sizeRepo = sizeRepo;
        }

        public async Task<ResultModel> CreateProductItems(ProductItemCreateModel productItemCreateModel, string token)
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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
                if (productItemCreateModel.Images == null || !productItemCreateModel.Images.Any())
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Image not found";
                    return result;
                }
                var prodItem = new TblProductItem()
                {
                    Id = Guid.NewGuid(),
                    Name = productItemCreateModel.Name,
                    SalePrice = productItemCreateModel.SalePrice,
                    Status = productItemCreateModel.Status,
                    Description = productItemCreateModel.Description,
                    ProductId = productItemCreateModel.ProductId,
                    SizeId = productItemCreateModel.SizeId,
                    Quantity = productItemCreateModel.Quantity,
                    Type = productItemCreateModel.Type,
                    RentPrice = productItemCreateModel.RentPrice,
                    Content = productItemCreateModel.Content
                };

                var insertResult = await _proItemRepo.Insert(prodItem);

                List<string> urlList = new();
                if (productItemCreateModel.Images != null)
                {
                    var IMGsUpload = await _imgService.UploadImages(productItemCreateModel.Images);
                    
                    if(IMGsUpload.IsSuccess == true)
                    {
                        urlList = (List<string>)IMGsUpload.Data;
                        foreach (string url in urlList)
                        {
                            var newImgProdItem = new TblImage()
                            {
                                Id = Guid.NewGuid(),
                                ImageUrl = url,
                                ProductItemId = prodItem.Id
                            };
                            await _imageRepo.Insert(newImgProdItem);
                        }
                    }
                }
                var prodItemRes = new ProductItemModel()
                {
                    Id = prodItem.Id,
                    Name = productItemCreateModel.Name,
                    SalePrice = productItemCreateModel.SalePrice,
                    Status = productItemCreateModel.Status,
                    Description = productItemCreateModel.Description,
                    ProductId = productItemCreateModel.ProductId,
                    Size = await _sizeRepo.Get(productItemCreateModel.SizeId),
                    Quantity = productItemCreateModel.Quantity,
                    Type = productItemCreateModel.Type,
                    RentPrice = productItemCreateModel.RentPrice,
                    Content = productItemCreateModel.Content,
                    ImgURLs = urlList
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = prodItemRes;
                result.Message = "Create new product item successfully";
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

        public async Task<ResultModel> GetProductItems(PaginationRequestModel pagingModel, Guid productID, Guid? sizeID, string? type, string? status)
        {
            var result = new ResultModel();
            try
            {
                if(sizeID == null) { sizeID = Guid.Parse("00000000-0000-0000-0000-000000000000"); }
                var listProductItem = await _proItemRepo.GetProductItems(pagingModel, productID, sizeID, type, status);
                if (listProductItem == null)
                {
                    result.Message = "Can not find any product item";
                    result.IsSuccess = true;
                    result.Code = 200;
                    return result;
                }
                List<ProductItemModel> listData = new(); 
                foreach (var pi in listProductItem.Results)
                {
                    var getProdItemIMGs = await _imageRepo.GetImgUrlProductItem(pi.Id);
                    var size = await _sizeRepo.Get(pi.SizeId);
                    var sizeRes = new SizeModel()
                    {
                        Id = size.Id,
                        SizeName = size.Name
                    };
                    List<string> prodItemIMGs = new();
                    foreach(var img in getProdItemIMGs)
                    {
                        prodItemIMGs.Add(img.ImageUrl);
                    }
                    var productItem = new ProductItemModel
                    {
                        Id = pi.Id,
                        Name = pi.Name,
                        SalePrice = pi.SalePrice,
                        Status = pi.Status,
                        Description = pi.Description,
                        ProductId = pi.ProductId,
                        Size = sizeRes,
                        Quantity = pi.Quantity,
                        Type = pi.Type,
                        RentPrice = pi.RentPrice,
                        Content = pi.Content,
                        ImgURLs = prodItemIMGs
                    };
                    listData.Add(productItem);
                }
                var paging = new PaginationResponseModel()
                    .PageSize(listProductItem.PageSize)
                    .CurPage(listProductItem.CurrentPage)
                    .RecordCount(listProductItem.RecordCount)
                    .PageCount(listProductItem.PageCount);

                var product = await _proRepo.queryAProductByProId(productID);
                var productIMG =  await _imageRepo.GetImgUrlProduct(product.Id);
                string prodIMG;
                if(productIMG != null) { prodIMG = productIMG.ImageUrl; ; } else { prodIMG = ""; }
                
                var productRes = new ProductModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Status = product.Status,
                    CategoryId = product.CategoryId,
                    ImgUrl = prodIMG,
                    IsForRent = product.IsForRent,
                    IsForSale = product.IsForSale

                };
                var category = await _categoryRepo.Get(product.CategoryId);
                var categoryIMG = await _imageRepo.GetImgUrlCategory(category.Id);
                var categoryRes = new CategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Status = category.Status,
                    ImgUrl = categoryIMG.ImageUrl,
                    Description = category.Description
                };
                var response = new ProductItemResponseResult()
                {
                    Paging = paging,
                    Category = categoryRes,
                    Product = productRes,
                    ProductItems          = listData,
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = response;
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

        public async Task<ResultModel> UpdateProductItem(ProductItemUpdateModel productItemUpdateModel, string token)
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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
                var productItemUpdate = await _proItemRepo.UpdateProductItem(productItemUpdateModel);

                if (productItemUpdate == false)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Can not find product item with ID: " + productItemUpdateModel.Id;
                    return result;
                }
                var newProdItem = await _proItemRepo.Get(productItemUpdateModel.Id);
                var getProdItemIMGs = await _imageRepo.GetImgUrlProductItem(newProdItem.Id);
                var size = await _sizeRepo.Get(newProdItem.SizeId);
                var sizeRes = new SizeModel()
                {
                    Id = size.Id,
                    SizeName = size.Name
                };
                List<string> prodItemIMGs = new();
                foreach (var img in getProdItemIMGs)
                {
                    prodItemIMGs.Add(img.ImageUrl);
                }
                var newProductItem = new ProductItemModel
                {
                    Id = newProdItem.Id,
                    Name = newProdItem.Name,
                    SalePrice = newProdItem.SalePrice,
                    Status = newProdItem.Status,
                    Description = newProdItem.Description,
                    ProductId = newProdItem.ProductId,
                    Size = sizeRes,
                    Quantity = newProdItem.Quantity,
                    Type = newProdItem.Type,
                    RentPrice = newProdItem.RentPrice,
                    Content = newProdItem.Content,
                    ImgURLs = prodItemIMGs
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = newProductItem;
                result.Message = "Product item updated without image change";
                if (productItemUpdate != false && productItemUpdateModel.Images != null)
                {
                    var productImgUpdate = await _imgService.UpdateImageProductItem(productItemUpdateModel.Id, productItemUpdateModel.Images);
                    if (productImgUpdate != null)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Data = newProductItem;
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
