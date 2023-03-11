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
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using System.Security.Claims;

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
        private readonly ISizeProductItemRepo _sizeProductItemRepo;
        public ProductItemService(ISizeProductItemRepo sizeProductItemRepo, ISizeRepo sizeRepo, IProductItemRepo proItemRepo, IProductRepo proRepo, IImageRepo imageRepo, IImageService imgService, ICategoryRepo categoryRepo)
        {
            _proItemRepo = proItemRepo;
            _imgService = imgService;
            _decodeToken = new DecodeToken();
            _imageRepo = imageRepo;
            _proRepo = proRepo;
            _categoryRepo = categoryRepo;
            _sizeRepo = sizeRepo;
            _sizeProductItemRepo = sizeProductItemRepo;
        }

        public async Task<ResultModel> CreateProductItem(string token, ProductItemInsertModel productItemInsertModel)
        {
            if (!String.IsNullOrEmpty(token))
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
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            var result = new ResultModel();
            try
            {
                TblProductItem productItemModel = new TblProductItem
                {
                    Id = Guid.NewGuid(),
                    Name = productItemInsertModel.Name,
                    Description = productItemInsertModel.Description,
                    Content = productItemInsertModel.Content,
                    ProductId = productItemInsertModel.ProductId,
                    Type = productItemInsertModel.Type,
                    
                };
                var insertProdItem = await _proItemRepo.Insert(productItemModel);
                if (insertProdItem == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Add product item failed.";
                    return result;
                }
                else {
                    TblImage tblImage = new TblImage
                    {
                        ImageUrl = productItemInsertModel.ImageURL,
                        ProductItemId = productItemModel.Id
                    };
                    await _imageRepo.Insert(tblImage);
                }

                if (productItemModel != null)
                {
                    var getProdItemImgURL = await _imageRepo.GetImgUrlProduct(productItemModel.Id);
                    string prodItemImgURL = "";
                    if (getProdItemImgURL != null)
                    {
                        prodItemImgURL = getProdItemImgURL.ImageUrl;
                    }
                    else
                    {
                        prodItemImgURL = "";
                    }
                    ProductItemResModel responseProdItem = new ProductItemResModel()
                    {
                        Id = productItemModel.Id,
                        Name = productItemModel.Name,
                        Description = productItemModel.Description,
                        Content = productItemModel.Content,
                        ProductId = productItemModel.ProductId,
                        Type = productItemModel.Type,
                        ImageURL = prodItemImgURL,
                    };
                    TblProduct productTbl = await _proRepo.Get(productItemModel.ProductId);
                    var getProdImgURL = await _imageRepo.GetImgUrlProduct(productItemModel.ProductId);
                    string prodImgURL = "";
                    if (getProdImgURL != null)
                    {
                        prodImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        prodImgURL = "";
                    }
                    ProductModel productModel = new ProductModel()
                    {
                        Id = productTbl.Id,
                        Name = productTbl.Name,
                        Description = productTbl.Description,
                        Status = productTbl.Status,
                        CategoryId = productTbl.CategoryId,
                        ImgUrl = prodImgURL,
                        IsForRent = productTbl.IsForRent,
                        IsForSale = productTbl.IsForSale
                    };
                    TblCategory categoryTBL = await _categoryRepo.Get(productModel.CategoryId);
                    var getCateImgURL = await _imageRepo.GetImgUrlCategory(categoryTBL.Id);
                    string cateImgURL = "";
                    if (getCateImgURL != null)
                    {
                        cateImgURL = getCateImgURL.ImageUrl;
                    }
                    else
                    {
                        cateImgURL = "";
                    }
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        Id = categoryTBL.Id,
                        Name = categoryTBL.Name,
                        Description = categoryTBL.Description,
                        Status = categoryTBL.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemCreateResponseResult productItemCreateResponseResult = new ProductItemCreateResponseResult()
                    {
                        Category = categoryModel,
                        Product = productModel,
                        ProductItems = responseProdItem
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = productItemCreateResponseResult;
                    result.Message = "Create product item success.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Add product item failed.";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> CreateProductItemDetail(string token, ProductItemDetailModel productItemDetailModel)
        {
            if (!String.IsNullOrEmpty(token))
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
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            var result = new ResultModel();
            try
            {
                var size = await _sizeRepo.Get(productItemDetailModel.SizeId);
                if (size == null )
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Size ID invalid.";
                    return result;
                }
                var proItem = await _proItemRepo.Get(productItemDetailModel.ProductItemID);
                if (proItem == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product item ID invalid.";
                    return result;
                }
                TblProductItemDetail tblProductItemDetail = new TblProductItemDetail
                {
                    Id = Guid.NewGuid(),
                    SizeId = productItemDetailModel.SizeId,
                    ProductItemId = productItemDetailModel.ProductItemID,
                    RentPrice = productItemDetailModel.RentPrice,
                    SalePrice = productItemDetailModel.SalePrice,
                    Quantity = productItemDetailModel.Quantity,
                    Status = productItemDetailModel.Status,
                };
                var insertSizeProdItem = await _sizeProductItemRepo.Insert(tblProductItemDetail);
                if (insertSizeProdItem == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create product item size failed.";
                    return result;
                }
                else
                {
                    foreach (string url in productItemDetailModel.ImagesUrls)
                    {
                        TblImage tblImage = new TblImage
                        {
                            ImageUrl = url,
                            ProductItemDetailId = tblProductItemDetail.Id,
                        };
                        await _imageRepo.Insert(tblImage);
                    }
                }
                var prodItem = await _proItemRepo.Get(productItemDetailModel.ProductItemID);
                if (prodItem != null)
                {
                    var sizeGet = await _sizeProductItemRepo.GetSizeProductItems(productItemDetailModel.ProductItemID, null);
                    var getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(productItemDetailModel.ProductItemID);
                    string prodItemImgURL = "";
                    if (getProdItemImgURL != null)
                    {
                        prodItemImgURL = getProdItemImgURL.ImageUrl;
                    }
                    else
                    {
                        prodItemImgURL = "";
                    }
                    ProductItemResModel productItemResModel = new ProductItemResModel()
                    {
                        Id = prodItem.Id,
                        Name = prodItem.Name,
                        Description = prodItem.Description,
                        Content = prodItem.Content,
                        ProductId = prodItem.ProductId,
                        Type = prodItem.Type,
                        ImageURL = prodItemImgURL,
                        ProductItemDetail = sizeGet
                    };
                    var productGet = await _proRepo.Get(productItemResModel.ProductId);
                    var getProdImgURL = await _imageRepo.GetImgUrlProduct(productItemResModel.ProductId);
                    string prodImgURL = "";
                    if (getProdImgURL != null)
                    {
                        prodImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        prodImgURL = "";
                    }
                    ProductModel productModel = new ProductModel()
                    {
                        Id = productGet.Id,
                        Name = productGet.Name,
                        Description = productGet.Description,
                        Status = productGet.Status,
                        CategoryId = productGet.CategoryId,
                        ImgUrl = prodImgURL,
                        IsForRent = productGet.IsForRent,
                        IsForSale = productGet.IsForSale
                    };
                    ///
                    var cateGet = await _categoryRepo.Get(productModel.CategoryId);
                    var getCateImgURL = await _imageRepo.GetImgUrlCategory(cateGet.Id);
                    string cateImgURL = "";
                    if (getCateImgURL != null)
                    {
                        cateImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        cateImgURL = "";
                    }
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemDetailResponseResult productItemDetailResponseResult = new ProductItemDetailResponseResult
                    {
                        Category = categoryModel,
                        Product = productModel,
                        ProductItem = productItemResModel
                    };
                    result.Message = "Get Detail successful.";
                    result.IsSuccess = true;
                    result.Data = productItemDetailResponseResult;
                    result.Code = 200;
                    return result;
                }
                else
                {
                    result.Message = "Get Detail failed.";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;

            }
        }

        public async Task<ResultModel> GetDetailProductItem(Guid productItemID, string? sizeProductItemStatus)
        {
            var result = new ResultModel();
            if ( productItemID != Guid.Empty)
            {
                var prodItem = await _proItemRepo.Get(productItemID);
                if (prodItem != null)
                {
                    var sizeGet = await _sizeProductItemRepo.GetSizeProductItems(productItemID, sizeProductItemStatus);
                    var getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(productItemID);
                    string prodItemImgURL = "";
                    if (getProdItemImgURL != null)
                    {
                        prodItemImgURL = getProdItemImgURL.ImageUrl;
                    }
                    else
                    {
                        prodItemImgURL = "";
                    }
                    ProductItemResModel productItemResModel = new ProductItemResModel()
                    {
                        Id = prodItem.Id,
                        Name = prodItem.Name,
                        Description = prodItem.Description,
                        Content = prodItem.Content,
                        ProductId = prodItem.ProductId,
                        Type = prodItem.Type,
                        ImageURL = prodItemImgURL,
                        ProductItemDetail = sizeGet
                    };
                    var productGet = await _proRepo.Get(productItemResModel.ProductId);
                    var getProdImgURL = await _imageRepo.GetImgUrlProduct(productItemResModel.ProductId);
                    string prodImgURL = "";
                    if (getProdImgURL != null)
                    {
                        prodImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        prodImgURL = "";
                    }
                    ProductModel productModel = new ProductModel()
                    {
                        Id = productGet.Id,
                        Name = productGet.Name,
                        Description = productGet.Description,
                        Status = productGet.Status,
                        CategoryId = productGet.CategoryId,
                        ImgUrl = prodImgURL,
                        IsForRent = productGet.IsForRent,
                        IsForSale = productGet.IsForSale
                    };
                    ///
                    var cateGet = await _categoryRepo.Get(productModel.CategoryId);
                    var getCateImgURL = await _imageRepo.GetImgUrlCategory(cateGet.Id);
                    string cateImgURL = "";
                    if (getCateImgURL != null)
                    {
                        cateImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        cateImgURL = "";
                    }
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemDetailResponseResult productItemDetailResponseResult = new ProductItemDetailResponseResult
                    {
                        Category = categoryModel,
                        Product = productModel,
                        ProductItem = productItemResModel
                    };
                    result.Message = "Get Detail successful.";
                    result.IsSuccess = true;
                    result.Data = productItemDetailResponseResult;
                    result.Code = 200;
                    return result;
                }
                else
                {
                    result.Message = "Get Detail failed.";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }
            }
            else
            {
                result.Message = "Get Detail failed.";
                result.IsSuccess = false;
                result.Code = 400;
                return result;
            }
        }

        public async Task<ResultModel> GetProductItem(PaginationRequestModel pagingModel, Guid productID, string? status, string? type)
        {
            var result = new ResultModel();
            try
            {
                var productGet = await _proRepo.Get(productID);
                if (productID == Guid.Empty || productGet == null)
                {
                    result.Message = "Can not find product.";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }
                var prodItemList = await _proItemRepo.GetProductItemByType(pagingModel, productID, type);
                if (prodItemList != null)
                {
                    List<ProductItemResModel> dataList = new List<ProductItemResModel>();
                    foreach (var pi in prodItemList.Results)
                    {
                        var sizeGet = await _sizeProductItemRepo.GetSizeProductItems(pi.Id, status);
                        var getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(pi.Id);
                        string prodItemImgURL = "";
                        if (getProdItemImgURL != null)
                        {
                            prodItemImgURL = getProdItemImgURL.ImageUrl;
                        }
                        else
                        {
                            prodItemImgURL = "";
                        }
                        
                            ProductItemResModel pItem = new ProductItemResModel()
                            {
                                Id = pi.Id,
                                Name = pi.Name,
                                Description = pi.Description,
                                Content = pi.Content,
                                ProductId = pi.ProductId,
                                Type = pi.Type,
                                ImageURL = prodItemImgURL,
                                ProductItemDetail = sizeGet
                            };

                            dataList.Add(pItem);
                       

                    }
                    ///
                    var paging = new PaginationResponseModel()
                    .PageSize(prodItemList.PageSize)
                    .CurPage(prodItemList.CurrentPage)
                    .RecordCount(prodItemList.RecordCount)
                    .PageCount(prodItemList.PageCount);
                    ///

                    var getProdImgURL = await _imageRepo.GetImgUrlProduct(productID);
                    string prodImgURL = "";
                    if (getProdImgURL != null)
                    {
                        prodImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        prodImgURL = "";
                    }
                    ProductModel productModel = new ProductModel()
                    {
                        Id = productGet.Id,
                        Name = productGet.Name,
                        Description = productGet.Description,
                        Status = productGet.Status,
                        CategoryId = productGet.CategoryId,
                        ImgUrl = prodImgURL,
                        IsForRent = productGet.IsForRent,
                        IsForSale = productGet.IsForSale
                    };
                    ///
                    var cateGet = await _categoryRepo.Get(productModel.CategoryId);
                    var getCateImgURL = await _imageRepo.GetImgUrlCategory(cateGet.Id);
                    string cateImgURL = "";
                    if (getCateImgURL != null)
                    {
                        cateImgURL = getProdImgURL.ImageUrl;
                    }
                    else
                    {
                        cateImgURL = "";
                    }
                    CategoryModel categoryModel = new CategoryModel()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemGetResponseResult productItemGetResponseResult = new ProductItemGetResponseResult()
                    {
                        Paging = paging,
                        Category = categoryModel,
                        Product = productModel,
                        ProductItems = dataList

                    };
                    result.Message = "Get list successful.";
                    result.IsSuccess = true;
                    result.Data = productItemGetResponseResult;
                    result.Code = 200;
                    return result;

                }
                else
                {
                    result.Message = "List empty.";
                    result.IsSuccess = true;
                    result.Code = 200;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }
        }

        public async Task<ResultModel> UpdateProductItem(string token, ProductItemUpdateModel productItemModel)
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
                var updateProItem = await _proItemRepo.UpdateProductItem(productItemModel);
                var oldImage = await _imageRepo.GetImgUrlProductItem(productItemModel.Id);
                if (oldImage != null)
                {
                    List<string> url = new List<string> { oldImage.ImageUrl};
                    var delete = await _imgService.DeleteImagesByURLs(url);
                }
                var updateNewImage = await _imageRepo.UpdateImgForProductItem(productItemModel.Id, productItemModel.ImageURL);
                if (updateProItem == true)
                {
                    var updateResult = await _proItemRepo.Get(productItemModel.Id);
                    var sizeGet = await _sizeProductItemRepo.GetSizeProductItems(updateResult.Id, null);
                    ProductItemResModel upResult = new ProductItemResModel
                    {
                        Id = updateResult.Id,
                        Name = updateResult.Name,
                        Description = updateResult.Description,
                        ProductId = updateResult.ProductId,
                        Type = updateResult.Type,
                        ProductItemDetail = sizeGet
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = upResult;
                    result.Message = "Product item updated.";
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product item update failed.";
                    return result;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                return result;
            }

        }

        public async Task<ResultModel> UpdateProductItemDetail(string token, ProductItemDetailModel productItemDetailModel)
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
                var updateProItem = await _sizeProductItemRepo.UpdateSizeProductItem(productItemDetailModel);
                if (updateProItem == true)
                {
                    var oldImgs = await _imageRepo.GetImgUrlProductItemDetail(productItemDetailModel.Id);
                    if (oldImgs.Any())
                    {
                        await _imgService.DeleteImagesByURLs(oldImgs);
                    }
                    foreach (string url in productItemDetailModel.ImagesUrls)
                    {
                        var updateImg = await _imageRepo.UpdateImgForProductItemDetail(productItemDetailModel.Id,productItemDetailModel.ImagesUrls);
                    }
                    var updateResult = await _sizeProductItemRepo.Get(productItemDetailModel.Id);
                    var sizeGet = await _sizeRepo.Get(updateResult.SizeId);
                    var imgGet = await _imageRepo.GetImgUrlProductItemDetail(updateResult.Id);
                    var size = new SizeResModel()
                    {
                        Id = sizeGet.Id,
                        SizeName = sizeGet.Name
                    };
                    ProductItemDetailResModel upResult = new ProductItemDetailResModel()
                    {
                        Id = updateResult.Id,
                        Size = size,
                        RentPrice = updateResult.RentPrice,
                        SalePrice = updateResult.SalePrice,
                        Quantity = updateResult.Quantity,
                        Status = updateResult.Status,
                        ImagesURL = imgGet
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = upResult;
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product item update failed.";
                    return result;
                }
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
