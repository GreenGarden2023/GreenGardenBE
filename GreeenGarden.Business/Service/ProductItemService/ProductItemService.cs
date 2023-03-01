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
using GreeenGarden.Data.Models.SizeProductItemModel;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
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
                if (!productItemInsertModel.sizeModelList.Any())
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Please enter atleast 1 product item size.";
                    return result;
                }
                TblProductItem productItemModel = new TblProductItem
                {
                    Id = Guid.NewGuid(),
                    Name = productItemInsertModel.Name,
                    Description = productItemInsertModel.Description,
                    ProductId = productItemInsertModel.ProductId,
                    Type = productItemInsertModel.Type
                };
                var insertProdItem = await _proItemRepo.Insert(productItemModel);
                List<SizeProductItemResModel> listSizeProductItemModel = new List<SizeProductItemResModel>();
                if (insertProdItem == Guid.Empty)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Create product item failed.";
                    return result;
                }
                else
                {
                    foreach (SizeProductItemInsertModel sizeModel in productItemInsertModel.sizeModelList)
                    {
                        TblSizeProductItem sizeProductItem = new TblSizeProductItem
                        {
                            Id = Guid.NewGuid(),
                            Name = sizeModel.Name,
                            SizeId = sizeModel.SizeId,
                            ProductItemId = productItemModel.ProductId,
                            RentPrice = sizeModel.RentPrice,
                            SalePrice = sizeModel.SalePrice,
                            Quantity = sizeModel.Quantity,
                            Content = sizeModel.Content,
                            Status = sizeModel.Status
                        };
                        var insertSizeProdItem = await _sizeProductItemRepo.Insert(sizeProductItem);
                        if (insertSizeProdItem == Guid.Empty)
                        {
                            var sizeProductItemURL = await _imgService.UploadImages(sizeModel.Images);
                            if (sizeProductItemURL.Code == 200)
                            {
                                foreach (string url in (List<string>)sizeProductItemURL.Data)
                                {
                                    TblImage tblImage = new TblImage()
                                    {
                                        ImageUrl = url,
                                        SizeProductItemId = sizeProductItem.Id
                                    };
                                    await _imageRepo.Insert(tblImage);
                                }
                            }



                            result.IsSuccess = false;
                            result.Code = 400;
                            result.Message = "Add product item size: " + sizeModel.Name + " failed.";
                            return result;
                        }
                        else
                        {
                            SizeProductItemResModel sizeProductItemModel = new SizeProductItemResModel()
                            {
                                Id = sizeProductItem.Id,
                                Name = sizeProductItem.Name,
                                SizeId = sizeProductItem.SizeId,
                                ProductItemId = sizeProductItem.ProductItemId,
                                RentPrice = sizeProductItem.RentPrice,
                                SalePrice = sizeProductItem.SalePrice,
                                Quantity = sizeProductItem.Quantity,
                                Content = sizeProductItem.Content,
                                Status = sizeProductItem.Status
                            };
                            listSizeProductItemModel.Add(sizeProductItemModel);
                        }
                    }
                }
                if (productItemModel != null && !listSizeProductItemModel.Any())
                {
                    ProductItemResModel responseProdItem = new ProductItemResModel()
                    {
                        Id = productItemModel.Id,
                        Name = productItemModel.Name,
                        Description = productItemModel.Description,
                        ProductId = productItemModel.ProductId,
                        Type = productItemModel.Type,
                        sizeModelList = listSizeProductItemModel
                    };
                    TblProduct productTbl = await _proRepo.Get(productItemModel.ProductId);
                    var getProdImgURL = await _imageRepo.GetImgUrlProduct(productItemModel.Id);
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
                    var getCateImgURL = await _imageRepo.GetImgUrlProduct(productItemModel.Id);
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
                        Id = productTbl.Id,
                        Name = productTbl.Name,
                        Description = productTbl.Description,
                        Status = productTbl.Status,
                        ImgUrl = prodImgURL,

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

        public async Task<ResultModel> GetProductItem(PaginationRequestModel pagingModel, Guid productID, string? status, string? type)
        {
            var result = new ResultModel();
            try
            {
                var prodItemList = await _proItemRepo.GetProductItemByType(pagingModel, type);
                if (prodItemList != null)
                {
                    List<ProductItemResModel> dataList = new List<ProductItemResModel>();
                    foreach (var pi in prodItemList.Results)
                    {
                        var sizeGet = await _sizeProductItemRepo.GetSizeProductItems(pi.Id, status);
                        if (sizeGet.Any())
                        {
                            ProductItemResModel pItem = new ProductItemResModel()
                            {
                                Id = pi.Id,
                                Name = pi.Name,
                                Description = pi.Description,
                                ProductId = pi.ProductId,
                                Type = pi.Type,
                                sizeModelList = sizeGet
                            };

                            dataList.Add(pItem);
                        }

                    }
                    ///
                    var paging = new PaginationResponseModel()
                    .PageSize(prodItemList.PageSize)
                    .CurPage(prodItemList.CurrentPage)
                    .RecordCount(prodItemList.RecordCount)
                    .PageCount(prodItemList.PageCount);
                    ///
                    var productGet = await _proRepo.Get(productID);
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
                if (updateProItem == true)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = productItemModel;
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

        public async Task<ResultModel> UpdateSizeProductItem(string token, SizeProductItemUpdateModel sizeProductItemResModel)
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
                var updateProItem = await _sizeProductItemRepo.UpdateSizeProductItem(sizeProductItemResModel);
                if (updateProItem == true)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = sizeProductItemResModel;
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
    }
}
