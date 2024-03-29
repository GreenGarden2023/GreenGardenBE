﻿using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public interface IProductItemRepo : IRepository<TblProductItem>
    {
        Task<Page<TblProductItem>> GetProductItemByType(PaginationRequestModel paginationRequestModel, Guid productId, string? type);
        Task<Page<TblProductItem>> GetProductItemByTypeByManager(PaginationRequestModel paginationRequestModel, Guid productId, string? type);
        Task<bool> UpdateProductItem(ProductItemModel productItemModel);
        Task<bool> ChangeStatus(ProductItemDetailUpdateStatusModel model);
        Task<Page<TblProductItem>> searchProductItem(Guid productID, PaginationRequestModel pagingModel);
        Task<List<TblProductItem>> GetItemsByItemDetail(List<TblProductItemDetail> itemDetails);
    }
}
