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
using GreeenGarden.Data.Models.SizeModel;
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
        private readonly IProductItemDetailRepo _productItemDetailRepo;
        public ProductItemService(IProductItemDetailRepo sizeProductItemRepo, ISizeRepo sizeRepo, IProductItemRepo proItemRepo, IProductRepo proRepo, IImageRepo imageRepo, IImageService imgService, ICategoryRepo categoryRepo)
        {
            _proItemRepo = proItemRepo;
            _imgService = imgService;
            _decodeToken = new DecodeToken();
            _imageRepo = imageRepo;
            _proRepo = proRepo;
            _categoryRepo = categoryRepo;
            _sizeRepo = sizeRepo;
            _productItemDetailRepo = sizeProductItemRepo;
        }

        public async Task<ResultModel> ChangeStatusProductItemDetail(string token, ProductItemDetailUpdateStatusModel model)
        {
            ResultModel result = new();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                result.IsSuccess = await _proItemRepo.ChangeStatus(model);


            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateProductItem(string token, ProductItemModel productItemInsertModel)
        {
            if (!string.IsNullOrEmpty(token))
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
            ResultModel result = new();
            try
            {
                TblProductItem productItemModel = new()
                {
                    Id = Guid.NewGuid(),
                    Name = productItemInsertModel.Name,
                    Description = productItemInsertModel.Description,
                    Content = productItemInsertModel.Content,
                    ProductId = (Guid)productItemInsertModel.ProductId,
                    Type = productItemInsertModel.Type,

                };
                Guid insertProdItem = await _proItemRepo.Insert(productItemModel);
                if (insertProdItem == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Add product item failed.";
                    return result;
                }
                else
                {
                    TblImage tblImage = new()
                    {
                        ImageUrl = productItemInsertModel.ImageURL,
                        ProductItemId = productItemModel.Id
                    };
                    _ = await _imageRepo.Insert(tblImage);
                }

                if (productItemModel != null)
                {
                    TblImage getProdItemImgURL = await _imageRepo.GetImgUrlProduct(productItemModel.Id);
                    string prodItemImgURL = getProdItemImgURL != null ? getProdItemImgURL.ImageUrl : "";
                    ProductItemResModel responseProdItem = new()
                    {
                        Id = productItemModel.Id,
                        Name = productItemModel.Name,
                        Description = productItemModel.Description,
                        Content = productItemModel.Content,
                        ProductId = productItemModel.ProductId,
                        Type = productItemModel.Type,
                        ImageURL = prodItemImgURL,
                    };

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = responseProdItem;
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
            if (!string.IsNullOrEmpty(token))
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
            ResultModel result = new();
            try
            {
                TblSize? size = await _sizeRepo.Get(productItemDetailModel.SizeId);
                if (size == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Size ID invalid.";
                    return result;
                }
                TblProductItem? proItem = await _proItemRepo.Get(productItemDetailModel.ProductItemID);
                if (proItem == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Product item ID invalid.";
                    return result;
                }
                TblProductItemDetail tblProductItemDetail = new()
                {
                    Id = Guid.NewGuid(),
                    SizeId = productItemDetailModel.SizeId,
                    ProductItemId = productItemDetailModel.ProductItemID,
                    RentPrice = productItemDetailModel.RentPrice,
                    SalePrice = productItemDetailModel.SalePrice,
                    Quantity = productItemDetailModel.Quantity,
                    TransportFee = productItemDetailModel.TransportFee,
                    Status = productItemDetailModel.Status,
                };

                Guid insertSizeProdItem = await _productItemDetailRepo.Insert(tblProductItemDetail);
                if (insertSizeProdItem == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create Product Item Detail failed.";
                    return result;
                }
                else
                {
                    foreach (string url in productItemDetailModel.ImagesUrls)
                    {
                        TblImage tblImage = new()
                        {
                            ImageUrl = url,
                            ProductItemDetailId = tblProductItemDetail.Id,
                        };
                        _ = await _imageRepo.Insert(tblImage);
                    }
                }
                TblProductItem? prodItem = await _proItemRepo.Get(productItemDetailModel.ProductItemID);
                if (prodItem != null)
                {
                    ProductItemDetailModel resModel = new()
                    {
                        Id = tblProductItemDetail.Id,
                        SizeId = tblProductItemDetail.SizeId,
                        ProductItemID = tblProductItemDetail.ProductItemId,
                        RentPrice = tblProductItemDetail.RentPrice,
                        SalePrice = tblProductItemDetail.SalePrice,
                        Quantity = tblProductItemDetail.Quantity,
                        Status = tblProductItemDetail.Status,
                        TransportFee = tblProductItemDetail.TransportFee,
                        ImagesUrls = productItemDetailModel.ImagesUrls
                    };
                    result.Message = "Create Product Item Detail successful.";
                    result.IsSuccess = true;
                    result.Data = resModel;
                    result.Code = 200;
                    return result;
                }
                else
                {
                    result.Message = "Create Product Item Detail failed.";
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
            ResultModel result = new();
            if (productItemID != Guid.Empty)
            {
                TblProductItem? prodItem = await _proItemRepo.Get(productItemID);
                if (prodItem != null)
                {
                    List<ProductItemDetailResModel> sizeGet = await _productItemDetailRepo.GetSizeProductItems(productItemID, sizeProductItemStatus);
                    TblImage getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(productItemID);
                    string prodItemImgURL = getProdItemImgURL != null ? getProdItemImgURL.ImageUrl : "";
                    ProductItemResModel productItemResModel = new()
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
                    TblProduct? productGet = await _proRepo.Get(productItemResModel.ProductId);
                    TblImage? getProdImgURL = await _imageRepo.GetImgUrlProduct(productItemResModel.ProductId);
                    string prodImgURL = getProdImgURL != null ? getProdImgURL.ImageUrl : "";
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
                    string cateImgURL = getCateImgURL != null ? getProdImgURL.ImageUrl : "";
                    CategoryModel categoryModel = new()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemDetailResponseResult productItemDetailResponseResult = new()
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
            ResultModel result = new();
            try
            {
                TblProduct? productGet = await _proRepo.Get(productID);
                if (productID == Guid.Empty || productGet == null)
                {
                    result.Message = "Can not find product.";
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }
                EntityFrameworkPaginateCore.Page<TblProductItem> prodItemList = await _proItemRepo.GetProductItemByType(pagingModel, productID, type);
                if (prodItemList != null)
                {
                    List<ProductItemResModel> dataList = new();
                    foreach (TblProductItem pi in prodItemList.Results)
                    {
                        List<ProductItemDetailResModel> sizeGet = await _productItemDetailRepo.GetSizeProductItems(pi.Id, status);
                        TblImage getProdItemImgURL = await _imageRepo.GetImgUrlProductItem(pi.Id);
                        string prodItemImgURL = getProdItemImgURL != null ? getProdItemImgURL.ImageUrl : "";
                        ProductItemResModel pItem = new()
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
                    PaginationResponseModel paging = new PaginationResponseModel()
                    .PageSize(prodItemList.PageSize)
                    .CurPage(prodItemList.CurrentPage)
                    .RecordCount(prodItemList.RecordCount)
                    .PageCount(prodItemList.PageCount);
                    ///

                    TblImage? getProdImgURL = await _imageRepo.GetImgUrlProduct(productID);
                    string prodImgURL = getProdImgURL != null ? getProdImgURL.ImageUrl : "";
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
                    string cateImgURL = getCateImgURL != null ? getProdImgURL.ImageUrl : "";
                    CategoryModel categoryModel = new()
                    {
                        Id = cateGet.Id,
                        Name = cateGet.Name,
                        Description = cateGet.Description,
                        Status = cateGet.Status,
                        ImgUrl = cateImgURL,

                    };
                    ProductItemGetResponseResult productItemGetResponseResult = new()
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

        public async Task<ResultModel> UpdateProductItem(string token, ProductItemModel productItemModel)
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
                        Message = "User not allowed"
                    };
                }
                bool updateProItem = await _proItemRepo.UpdateProductItem(productItemModel);

                TblImage updateNewImage = await _imageRepo.UpdateImgForProductItem(productItemModel.Id, productItemModel.ImageURL);
                if (updateProItem == true)
                {
                    TblProductItem? updateResult = await _proItemRepo.Get(productItemModel.Id);
                    List<ProductItemDetailResModel> sizeGet = await _productItemDetailRepo.GetSizeProductItems(updateResult.Id, null);
                    ProductItemResModel upResult = new()
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
                        Message = "User not allowed"
                    };
                }
                bool updateProItem = await _productItemDetailRepo.UpdateSizeProductItem(productItemDetailModel);
                if (updateProItem == true)
                {
                    if (productItemDetailModel.ImagesUrls != null)
                    {
                        foreach (string url in productItemDetailModel.ImagesUrls)
                        {
                            bool updateImg = await _imageRepo.UpdateImgForProductItemDetail((Guid)productItemDetailModel.Id, productItemDetailModel.ImagesUrls);
                        }
                    }
                    TblProductItemDetail? updateResult = await _productItemDetailRepo.Get((Guid)productItemDetailModel.Id);
                    TblSize? sizeGet = await _sizeRepo.Get(updateResult.SizeId);
                    List<string> imgGet = await _imageRepo.GetImgUrlProductItemDetail(updateResult.Id);
                    SizeResModel size = new()
                    {
                        Id = sizeGet.Id,
                        SizeName = sizeGet.Name
                    };
                    ProductItemDetailResModel upResult = new()
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
