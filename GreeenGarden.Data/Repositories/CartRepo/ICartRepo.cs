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
        Task<TblCart> GetCart(Guid UserID, bool? isForRent);
        Task<TblSizeProductItem> GetSizeProductItem(Guid? SizeProductItemID);
        Task<TblCartDetail> AddProductItemToCart(TblCartDetail model);
        Task<List<TblCartDetail>> GetListCartDetail(Guid cartID);
        Task<bool> RemoveCartDetail(TblCartDetail model);

    }
}
