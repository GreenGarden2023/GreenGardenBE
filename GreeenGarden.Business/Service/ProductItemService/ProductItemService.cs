using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CategoryModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SubProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ProductItemService
{
    public class ProductItemService : IProductItemService
    {
        //private readonly IMapper _mapper;
        private readonly IProductItemRepo _proItemRepo;
        private readonly IImageService _imgService;
        private readonly IImageRepo _imageRepo;
        private readonly ISubProductRepo _subRepo;
        private readonly IProductRepo _proRepo;
        public ProductItemService(/*IMapper mapper,*/ IProductItemRepo proItemRepo, IProductRepo proRepo, ISubProductRepo subRepo, IImageRepo imageRepo, IImageService imgService)
        {
            //_mapper = mapper;
            _proItemRepo= proItemRepo;
            _imgService = imgService;
            _imageRepo = imageRepo;
            _subRepo = subRepo;
            _proRepo = proRepo;
        }

        public async Task<ResultModel> getAllProductItemByProductItemSize(PaginationRequestModel pagingModel, Guid productSizeId)
        {
            var result = new ResultModel();
            try
            {
                var listItemBySize = _proItemRepo.queryAllItemByProductSize(pagingModel, productSizeId);
                if (listItemBySize == null)
                {
                    result.Message = "Don't have any item in size!";
                }


                List<ProductItemModel> dataList = new List<ProductItemModel>();
                foreach (var p in listItemBySize.Results)
                {
                    var ProductItemToShow = new ProductItemModel
                    {
                        id= p.Id,
                        name = p.Name,
                        price = p.Price,
                        status = p.Status,
                        description = p.Description,
                        subProductId = p.SubProductId,
                        imgUrl = _proItemRepo.getImgByProductItem(p.Id)
                    };
                    dataList.Add(ProductItemToShow);
                }
                var paging = new PaginationResponseModel()
                    .PageSize(listItemBySize.PageSize)
                    .CurPage(listItemBySize.CurrentPage)
                    .RecordCount(listItemBySize.RecordCount)
                    .PageCount(listItemBySize.PageCount);

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

        public async Task<ResultModel> getSizesOfProduct(PaginationRequestModel pagingModel, Guid productId)
        {
            var result = new ResultModel();
            try
            {
                var listProductSize = _proItemRepo.queryAllSizeByProduct(pagingModel, productId);
                if (listProductSize == null)
                {
                    result.Message = "Don't have any size of this product";
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listProductSize;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getDetailItem(Guid productItemId)
        {
            var result = new ResultModel();
            try
            {
                var productItem = _proItemRepo.queryDetailItemByProductSize(productItemId);
                if (productItem == null)
                {
                    result.Message = "Item does not exist!";
                }

                var response = new ProductItemModel()
                {
                    id = productItemId,
                    name = productItem.FirstOrDefault().Name,
                    price = productItem.FirstOrDefault()?.Price,
                    status = productItem.FirstOrDefault().Status,
                    description = productItem.FirstOrDefault().Description,
                    subProductId = productItem.FirstOrDefault().SubProductId,
                    imgUrl = _proItemRepo.getImgByProductItem(productItemId)
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

        public async Task<ResultModel> createProductItem(ProductItemCreateRequestModel model, string token)
        {
            var result = new ResultModel();

            var subProduct = await _subRepo.querySubAndSize(model.subProductId);
            if (subProduct.size.Equals(Size.UNIQUE))
            {
                var newProductItem = new TblProductItem()
                {
                    Id = Guid.NewGuid(),
                    Name = model.name,
                    Description = model.description,
                    SubProductId = model.subProductId,
                    Price = model.price,
                    Status = Status.ACTIVE,
                };
                await _proItemRepo.Insert(newProductItem);

            }
            else
            {
                var newProductItem = new TblProductItem()
                {
                    Id = Guid.NewGuid(),
                    Name = model.name,
                    Description = model.description,
                    SubProductId = model.subProductId,
                    Price = null,
                    Status = Status.ACTIVE,
                };
                await _proItemRepo.Insert(newProductItem);
            }




            try
            {
                var newProductItem = new TblProductItem()
                {
                    Id = Guid.NewGuid(),
                    Name = model.name,
                    Description = model.description,
                    SubProductId = model.subProductId,
                    Price = model.price,
                    Status = Status.ACTIVE,
                };
                await _proItemRepo.Insert(newProductItem);

                foreach (var i in model.imgFile)
                {
                    var imgUploadUrl = await _imgService.UploadAnImage(i);
                    var newImgProduct = new TblImage()
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = imgUploadUrl.Data.ToString(),
                        ProductItemId = newProductItem.Id,
                    };
                    await _imageRepo.Insert(newImgProduct);
                }


                result.Code = 200;
                result.IsSuccess = true;
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
            return result;
        }
    }
}
