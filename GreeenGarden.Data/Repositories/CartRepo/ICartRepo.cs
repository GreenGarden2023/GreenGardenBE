using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.CartRepo
{
    public interface ICartRepo : IRepository<TblCart>
    {
        Task<TblUser> GetByUserName(string username);
        Task<CartShowModel> GetCartShow(Guid UserID);
        Task<TblCart> GetCart(Guid UserID);
        Task<List<TblCartDetail>> GetCartDetail(Guid CartID);
        Task<TblProductItem> GetProductItem(Guid? ProductItemID);
        Task<TblCartDetail> AddProductItemToCart(TblCartDetail model);
        Task<TblCart> UpdateCart(TblCart model);

        
    }
}
