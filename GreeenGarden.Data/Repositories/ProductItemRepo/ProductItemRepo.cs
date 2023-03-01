using System.Collections.Generic;
using System.Linq;
using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Models.ProductModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.ProductItemRepo
{
    public class ProductItemRepo : Repository<TblProductItem>, IProductItemRepo
    {
        private readonly GreenGardenDbContext _context;
        public ProductItemRepo( GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Page<TblProductItem>> GetProductItemByType(PaginationRequestModel paginationRequestModel, string? type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return await _context.TblProductItems.PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);

            }
            else
            {
                return await _context.TblProductItems.Where(x=> x.Type.Trim().ToLower().Equals(type.Trim().ToLower())).PaginateAsync(paginationRequestModel.curPage, paginationRequestModel.pageSize);
            }
        }
    }
    
}
