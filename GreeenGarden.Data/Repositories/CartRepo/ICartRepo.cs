﻿using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.CartRepo
{
    public interface ICartRepo : IRepository<TblCart>
    {

        Task<TblUser> GetByUserName(string username);
        Task<TblCart> GetCart(Guid UserID);
        Task<TblSizeProductItem> GetSizeProductItem(Guid? SizeProductItemID);
        Task<TblCartDetail> AddProductItemToCart(TblCartDetail model);
        Task<List<TblCartDetail>> GetListCartDetail(Guid cartID);
        Task<bool> RemoveCartDetail(TblCartDetail model);

    }
}
