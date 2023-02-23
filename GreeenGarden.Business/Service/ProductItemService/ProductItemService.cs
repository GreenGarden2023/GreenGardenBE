using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.SubProductRepo;
using Microsoft.AspNetCore.Http;

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
            _proItemRepo = proItemRepo;
            _imgService = imgService;
            _imageRepo = imageRepo;
            _subRepo = subRepo;
            _proRepo = proRepo;
        }

        public Task<ResultModel> CreateProductItems(ProductItemCreateModel productItemCreateModel)
        {
            throw new NotImplementedException();
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
                        SizeId = pi.SizeId,
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

                var response = new ResponseResult()
                {
                    Paging = paging,
                    Result = listData,
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
    }
}
