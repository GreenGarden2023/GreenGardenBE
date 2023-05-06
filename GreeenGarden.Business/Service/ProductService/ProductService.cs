using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using System.Security.Claims;

namespace GreeenGarden.Business.Service.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly IProductItemRepo _productItemRepo;
        private readonly DecodeToken _decodeToken;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        public ProductService(IProductRepo productRepo, IImageService imgService, 
            IImageRepo imageRepo, ICategoryRepo categoryRepo, IProductItemRepo productItemRepo, IProductItemDetailRepo productItemDetailRepo)
        {
            _productRepo = productRepo;
            _decodeToken = new DecodeToken();
            _imgService = imgService;
            _imageRepo = imageRepo;
            _categoryRepo = categoryRepo;
            _productItemRepo = productItemRepo;
            _productItemDetailRepo= productItemDetailRepo;

        }

        public async Task<ResultModel> createProduct(ProductCreateModel model, string token)
        {
            ResultModel result = new();
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
                if (string.IsNullOrEmpty(model.Name) || model.Name.Length < 2 || model.Name.Length > 50)
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
                    result.Message = "Product image not found";
                    return result;
                }
                TblProduct newProduct = new()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Status = Status.ACTIVE,
                    CategoryId = model.CategoryId,
                    IsForRent = model.IsForRent,
                    IsForSale = model.IsForSale
                };
                Guid insertResult = await _productRepo.Insert(newProduct);
                ProductModel productResModel = new()
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    Description = newProduct.Description,
                    Status = newProduct.Status,
                    CategoryId = newProduct.CategoryId,
                    IsForRent = newProduct.IsForRent,
                    IsForSale = newProduct.IsForSale

                };

                // create, update tbl img()

                ResultModel imgUploadUrl = await _imgService.UploadAnImage(model.ImgFile);
                productResModel.ImgUrl = imgUploadUrl.Data.ToString();
                if (imgUploadUrl != null)
                {
                    TblImage newimgCategory = new()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = imgUploadUrl.Data.ToString(),
                        ProductId = newProduct.Id,
                    };
                    _ = await _imageRepo.Insert(newimgCategory);

                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = productResModel;
                result.Message = "Create new product successfully";
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

        public async Task<ResultModel> getAllProductByCategoryStatus(PaginationRequestModel pagingModel, Guid categoryID, string? status, string? rentSale)
        {
            ResultModel result = new();
            try
            {
                EntityFrameworkPaginateCore.Page<TblProduct> listProdct = await _productRepo.queryAllProductByCategoryAndStatus(pagingModel, categoryID, status, rentSale);

                if (listProdct == null)
                {
                    result.Message = "List null";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }

                List<ProductModel> dataList = new();
                foreach (TblProduct p in listProdct.Results)
                {
                    TblImage img = await _imageRepo.GetImgUrlProduct(p.Id);
                    string? imgURL = img != null ? img.ImageUrl : "";
                    ProductModel productToShow = new()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Status = p.Status,
                        CategoryId = p.CategoryId,
                        ImgUrl = imgURL,
                        IsForSale = p.IsForSale,
                        IsForRent = p.IsForRent
                    };
                    dataList.Add(productToShow);
                }
                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listProdct.PageSize)
                    .CurPage(listProdct.CurrentPage)
                    .RecordCount(listProdct.RecordCount)
                    .PageCount(listProdct.PageCount);

                TblCategory getCategory = await _categoryRepo.selectDetailCategory(categoryID);
                if (status != null)
                {
                    if (getCategory.Status.ToLower() != Status.ACTIVE)
                    {
                        var category = new CategoryModel()
                        {
                            Id = getCategory.Id,
                            Name = getCategory.Name,
                            Status = getCategory.Status.ToLower(),
                            Description = getCategory.Description,
                            ImgUrl = _imageRepo.GetImgUrlCategory(categoryID).Result.ImageUrl
                        };

                        PaginationResponseModel pagingRes = new PaginationResponseModel()
                            .PageSize(pagingModel.pageSize)
                            .CurPage(pagingModel.curPage)
                            .RecordCount(0)
                            .PageCount(0);
                        result.Data = new ProductResponseResult()
                        {
                            Paging = pagingRes,
                            Category = category,
                            Result = new List<ProductModel>()
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Message = "Status of category is: " + category.Status.ToLower();
                        return result;

                    }

                }
                CategoryModel categoryModel = new()
                {
                    Id = getCategory.Id,
                    Name = getCategory.Name,
                    Status = getCategory.Status.ToLower(),
                    Description = getCategory.Description,
                    ImgUrl = _imageRepo.GetImgUrlCategory(categoryID).Result.ImageUrl
                };
                ProductResponseResult response = new()
                {
                    Paging = paging,
                    Category = categoryModel,
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
            ResultModel result = new();
            try
            {

                EntityFrameworkPaginateCore.Page<TblProduct>? listProdct = await _productRepo.queryAllProduct(pagingModel);

                if (listProdct == null)
                {
                    result.Message = "List null";
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = listProdct;
                    return result;
                }

                List<ProductModel> dataList = new();
                foreach (TblProduct p in listProdct.Results)
                {
                    TblImage img = await _imageRepo.GetImgUrlProduct(p.Id);
                    string? imgURL = img != null ? img.ImageUrl : "";
                    ProductModel productToShow = new()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Status = p.Status,
                        CategoryId = p.CategoryId,
                        ImgUrl = imgURL,
                        IsForRent = p.IsForRent,
                        IsForSale = p.IsForSale
                    };
                    dataList.Add(productToShow);
                }
                PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listProdct.PageSize)
                    .CurPage(listProdct.CurrentPage)
                    .RecordCount(listProdct.RecordCount)
                    .PageCount(listProdct.PageCount);


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

        public async Task<ResultModel> UpdateProduct(ProductUpdateModel productUpdateModel, string token)
        {
            ResultModel result = new();
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
                if (string.IsNullOrEmpty(productUpdateModel.Name) &&
                string.IsNullOrEmpty(productUpdateModel.Description) &&
                string.IsNullOrEmpty(productUpdateModel.Status) &&
                productUpdateModel.CategoryId == Guid.Empty &&
                 productUpdateModel.Image == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Please update atleast 1 parameter";
                    return result;
                }

                if (string.IsNullOrEmpty(productUpdateModel.Name) || productUpdateModel.Name.Length < 2 || productUpdateModel.Name.Length > 50)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Product name is greater than 2 and less than 50 characters";
                    return result;
                }

                if (string.IsNullOrEmpty(productUpdateModel.Status) || productUpdateModel.Status.Length < 2 || productUpdateModel.Status.Length > 50)
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

                bool productUpdate = await _productRepo.UpdateProduct(productUpdateModel);

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
                    ResultModel productImgUpdate = await _imgService.UpdateImageProduct(productUpdateModel.ID, productUpdateModel.Image);
                    if (productImgUpdate.Code == 200)
                    {
                        result.IsSuccess = true;
                        result.Code = 200;
                        result.Message = "Product updated with image change";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "Product updated failed";
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

        public async Task<ResultModel> ChangeStatus(string token, ProductUpdateStatusModel model)
        {
            ResultModel result = new();
            try
            {
                result.IsSuccess = await _productRepo.changeStatus(model);
                result.Message = "Update sucessfully";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> searchProductOrProductItem(PaginationRequestModel pagingModel, ProductSearchModel model)
        {
            var result = new ResultModel();
            try
            {
                if (model.productID == null && model.categoryID != null)
                {
                    Guid categoryIDToSearch = (Guid) model.categoryID;
                    var listProduct = await _productRepo.searchProductByCategoty(pagingModel.searchText, categoryIDToSearch, pagingModel);

                    if (listProduct == null)
                    {
                        result.Message = "Not found category";
                        result.IsSuccess = true;
                        result.Data = listProduct;
                        return result;
                    }

                    List<ProductModel> dataList = new();
                    foreach (TblProduct p in listProduct.Results)
                    {
                        TblImage img = await _imageRepo.GetImgUrlProduct(p.Id);
                        string? imgURL = img != null ? img.ImageUrl : "";
                        ProductModel productToShow = new()
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Status = p.Status,
                            CategoryId = p.CategoryId,
                            ImgUrl = imgURL,
                            IsForSale = p.IsForSale,
                            IsForRent = p.IsForRent
                        };
                        dataList.Add(productToShow);
                    }
                    PaginationResponseModel paging = new PaginationResponseModel()
                        .PageSize(listProduct.PageSize)
                        .CurPage(listProduct.CurrentPage)
                        .RecordCount(listProduct.RecordCount)
                        .PageCount(listProduct.PageCount)
                        .SearchText(pagingModel.searchText);

                    TblCategory getCategory = await _categoryRepo.selectDetailCategory(categoryIDToSearch);
                    CategoryModel categoryModel = new()
                    {
                        Id = getCategory.Id,
                        Name = getCategory.Name,
                        Status = getCategory.Status.ToLower(),
                        Description = getCategory.Description,
                        ImgUrl = _imageRepo.GetImgUrlCategory(categoryIDToSearch).Result.ImageUrl
                    };
                    ProductResponseResult response = new()
                    {
                        Paging = paging,
                        Category = categoryModel,
                        Result = dataList
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Search product successful.";
                    result.Data = response;
                    return result;
                }

                if (model.productID != null && model.categoryID == null)
                {
                    var productID = (Guid) model.productID;
                    var listProductItem = await _productItemRepo.searchProductItem(productID, pagingModel);
                    if (listProductItem == null)
                    {
                        result.Message = "Not found product";
                        result.IsSuccess = true;
                        result.Data = listProductItem;
                        return result;
                    }
                    List<ProductItemResModel> dataList = new();
                    foreach (TblProductItem pi in listProductItem.Results)
                    {
                        List<ProductItemDetailResModel> sizeGet = await _productItemDetailRepo.GetSizeProductItems(pi.Id, Status.ACTIVE);
                        TblImage getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(pi.Id);
                        string? prodItemImgURL = getProdItemImgURL != null ? getProdItemImgURL.ImageUrl : "";
                        ProductItemResModel pItem = new()
                        {
                            Id = pi.Id,
                            Name = pi.Name,
                            Description = pi.Description,
                            Content = pi.Content,
                            ProductId = pi.ProductId,
                            Type = pi.Type,
                            ImageURL = prodItemImgURL,
                            Rule = pi.Rule,
                            ProductItemDetail = sizeGet
                        };

                        dataList.Add(pItem);


                    }
                    ///
                    PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(listProductItem.PageSize)
                    .CurPage(listProductItem.CurrentPage)
                    .RecordCount(listProductItem.RecordCount)
                    .PageCount(listProductItem.PageCount)
                    .SearchText(pagingModel.searchText); ;
                    ///

                    TblImage? getProdImgURL = await _imageRepo.GetImgUrlProduct(productID);
                    string? prodImgURL = getProdImgURL != null ? getProdImgURL.ImageUrl : "";
                    TblProduct? productGet = await _productRepo.Get(productID);
                    ProductModel productModel = new()
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
                    TblCategory? cateGet = await _categoryRepo.Get(productModel.CategoryId);
                    TblImage getCateImgURL = await _imageRepo.GetImgUrlCategory(cateGet.Id);
                    string? cateImgURL = getCateImgURL != null ? getProdImgURL.ImageUrl : ""; CategoryModel categoryModel = new()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status.ToLower(),
                        ImgUrl = cateImgURL,

                    };
                    ProductItemGetResponseResult productItemGetResponseResult = new()
                    {
                        Paging = paging,
                        Category = categoryModel,
                        Product = productModel,
                        ProductItems = dataList

                    };
                    result.Message = "Search productItem successful.";
                    result.IsSuccess = true;
                    result.Data = productItemGetResponseResult;
                    result.Code = 200;
                    return result;
                }

                result.IsSuccess = true;
                result.Message = "Data null";
                result.Data = null;
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
