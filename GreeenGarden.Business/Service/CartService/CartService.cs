using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;

namespace GreeenGarden.Business.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ICartRepo _cartRepo;

        public CartService(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> AddToCart(string token, AddToCartModel model)
        {
            var result = new ResultModel();
            try
            {
                double? totalPriceCart = 0;
                var modelResponse = new CartShowModel();
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                if (await _cartRepo.GetCart(user.Id, model.isForRent) == null)
                {
                    var cartTemp = new TblCart()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Status = Status.ACTIVE,
                    };
                    await _cartRepo.Insert(cartTemp);
                }

                var cart = await _cartRepo.GetCart(user.Id, model.isForRent);
                var cartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                foreach (var item in cartDetail)
                {
                    await _cartRepo.RemoveCartDetail(item);
                }
                modelResponse.isForRent = model.isForRent;
                modelResponse.items = new List<ItemRequest>();
                foreach (var item in model.items)
                {
                    var sizeProductItem = await _cartRepo.GetSizeProductItem(item.sizeProductItemID);
                    if (sizeProductItem.Quantity < item.quantity || sizeProductItem.Status.ToLower() != Status.ACTIVE)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Sản phẩm " + item.sizeProductItemID + " đã hết trong kho!";
                        return result;
                    }

                    var newCartDetail = new TblCartDetail()
                    {
                        Id = Guid.NewGuid(),
                        SizeProductItemId = item.sizeProductItemID,
                        Quantity = item.quantity,
                        CartId = cart.Id
                    };
                    await _cartRepo.AddProductItemToCart(newCartDetail);
                    if (model.isForRent == true) totalPriceCart += sizeProductItem.RentPrice * item.quantity;
                    if (model.isForRent == false) totalPriceCart += sizeProductItem.SalePrice * item.quantity;
                    var ItemRequest = new ItemRequest();
                    ItemRequest.sizeProductItemID = item.sizeProductItemID;
                    ItemRequest.quantity = item.quantity;
                    if (model.isForRent == true) ItemRequest.unitPrice = sizeProductItem.RentPrice;
                    if (model.isForRent == false) ItemRequest.unitPrice = sizeProductItem.SalePrice;
                    modelResponse.items.Add(ItemRequest);
                }
                modelResponse.totalPrice = totalPriceCart;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCart(string token, bool isForRent)
        {
            var result = new ResultModel();
            try
            {
                var modelResponse = new CartShowModel();
                double? totalPrice = 0;
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                var cart = await _cartRepo.GetCart(user.Id, isForRent);
                if (cart == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                var listCartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                if (listCartDetail.Count == 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                modelResponse.items = new List<ItemRequest>();
                foreach (var item in listCartDetail)
                {
                    var sizeProductItem = await _cartRepo.GetSizeProductItem(item.SizeProductItemId);
                    var ItemRequest = new ItemRequest();
                    ItemRequest.quantity = item.Quantity;
                    ItemRequest.sizeProductItemID = sizeProductItem.Id;
                    if (isForRent == true) ItemRequest.unitPrice = sizeProductItem.RentPrice;
                    if (isForRent == false) ItemRequest.unitPrice = sizeProductItem.SalePrice;
                    modelResponse.items.Add(ItemRequest);
                    if (isForRent == true) totalPrice += sizeProductItem.RentPrice * item.Quantity;
                    if (isForRent == false) totalPrice += sizeProductItem.SalePrice * item.Quantity;
                }
                modelResponse.totalPrice = totalPrice;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
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
